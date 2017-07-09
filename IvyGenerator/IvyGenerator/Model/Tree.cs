using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using IvyGenerator.Utilities;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;

namespace IvyGenerator.Model
{
    public class Tree : ObservableObject
    {
        private LSystem lSys = null;

        public PhongMaterial RedMaterial { get; private set; }

        private float length = 5.0f;
        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                Generate(false);
                RaisePropertyChanged("Length");
            }
        }
        private float radius = 0.1f;
        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                Generate(false);
                RaisePropertyChanged("Radius");
            }
        }

        private float internalRadius;
        private float radiusReduction = 0.1f;
        public float RadiusReduction
        {
            get { return radiusReduction; }
            set
            {
                radiusReduction = value;
                Generate(false);
                RaisePropertyChanged("RadiusReduction");
            }
        }
        private float angle = 25.0f;
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                Generate(false);
                RaisePropertyChanged("Angle");
            }
        }

        public void LoadProject(SaveTreeMemento loadedData)
        {
            Angle = loadedData.Angle;
            Radius = loadedData.Radius;
            RadiusReduction = loadedData.RadiusReduction;
            Length = loadedData.Length;
            lSys = new LSystem(loadedData.RuleSet);
            lSys.SetCurrent(loadedData.Current);
        }

        public void SetRuleSet(RuleSet ruleSet)
        {
            lSys = new LSystem(ruleSet);
            Reset();
            Generate(false);
        }

        public string GetCurrent()
        {
            return lSys.Current;
        }

        public RuleSet GetRuleSet()
        {
            return lSys.RuleSet;
        }

        private Stack<Matrix> matrices = new Stack<Matrix>();
        private MeshBuilder treeGeom = new MeshBuilder();

        private MeshGeometry3D treeGeometry;
        public MeshGeometry3D TreeGeometry
        {
            get { return treeGeometry; }
            set
            {
                treeGeometry = value;
                RaisePropertyChanged("TreeGeometry");
            }
        }

        private Matrix currentMatrix = Matrix.Identity;

        public Tree()
        {
            //Rotate by 90 to face up
            currentMatrix *= Matrix.RotationAxis(Vector3.Right, (float)(90 * Math.PI / 180));
            TreeGeometry = null;
            RedMaterial = PhongMaterials.Red;
        }

        public void Generate(bool advanceGeneration = true)
        {
            if(advanceGeneration)
                lSys.Generate();
            currentMatrix = Matrix.RotationAxis(Vector3.Right, (float)(90 * Math.PI / 180));
            internalRadius = radius;
            string curr = lSys.Current;
            treeGeom = new MeshBuilder();
            foreach (char c in curr)
            {
                switch(c)
                {
                    case 'f'://Create branch
                        Line(length);
                        Translate(length);
                        break;
                    case 'g'://Move forward
                        Translate(length);
                        break;
                    case '+': //rotate X Axis
                        RotateX(-angle);
                        break;
                    case '-': //rotate X Axis
                        RotateX(angle);
                        break;
                    case '&': //rotate Y Axis
                        RotateY(-angle);
                        break;
                    case '^': //rotate Y Axis
                        RotateY(angle);
                        break;
                    case '<': //rotate Z Axis
                        RotateZ(-angle);
                        break;
                    case '>': //rotate Z Axis
                        RotateZ(angle);
                        break;
                    case '[':
                        PushMatrix();
                        break;
                    case ']':
                        PopMatrix();
                        break;
                }
            }

            var TreeMesh = treeGeom.ToMeshGeometry3D();
            TreeMesh.Colors = new Color4Collection(treeGeom.TextureCoordinates.Select(x => x.ToColor4()));
            TreeGeometry = TreeMesh;
        }

        public void Reset()
        {
            lSys.Reset();
            treeGeom = new MeshBuilder();
            TreeGeometry = null;
        }

        private void Line(float len)
        {
            //treeGeom.AddCylinder(currentMatrix.TranslationVector, currentMatrix.TranslationVector + currentMatrix.Forward * length, radius, 12, true, true);
            SharpDX.Vector3 p1 = currentMatrix.TranslationVector;
            SharpDX.Vector3 p2 = currentMatrix.TranslationVector + currentMatrix.Forward * length;

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();
            int sides = 6;

            float shapeAngle = 360.0f / sides;

            for (int i = 0; i < sides; i++)
            {
                Vector3 dir = currentMatrix.Up;
                Matrix rot = Matrix.RotationAxis(currentMatrix.Forward, MathUtil.DegreesToRadians(shapeAngle * i));
                dir = Vector3.TransformNormal(dir, rot);
                Vector3 v = p1 + dir * internalRadius;
                positions.Add(v);
                norms.Add((v - p1).Normalized());
                uvs.Add(Vector2.Zero);
            }

            internalRadius = (internalRadius - radiusReduction > 0.1) ? internalRadius - radiusReduction : 0.1f;
            for (int i = 0; i < sides; i++)
            {
                Vector3 dir = currentMatrix.Up;
                Matrix rot = Matrix.RotationAxis(currentMatrix.Forward, MathUtil.DegreesToRadians(shapeAngle * i));
                dir = Vector3.TransformNormal(dir, rot);
                Vector3 v = p2 + dir * internalRadius;
                positions.Add(v);
                norms.Add((v - p2).Normalized());
                uvs.Add(Vector2.Zero);
            }

            for (int i = 0; i < sides; i++)
            {
                int a = i;
                int b = i == sides - 1 ? 0 : i + 1;
                int c = i + sides;
                int d = (i == sides - 1) ? sides : i + sides + 1;

                indices.Add(a);
                indices.Add(c);
                indices.Add(b);
                indices.Add(b);
                indices.Add(c);
                indices.Add(d);
            }

            treeGeom.Append(positions, indices, norms, uvs);
        }

        private void Translate(float len)
        {
            currentMatrix *= Matrix.Translation(currentMatrix.Forward * len);
        }

        private void RotateZ(float angle)
        {
            Vector3 translationVector = currentMatrix.TranslationVector;
            currentMatrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            var a = angle * Math.PI / 180;
            currentMatrix *= Matrix.RotationAxis(currentMatrix.Forward, (float)a);
            currentMatrix *= Matrix.Translation(translationVector);
        }

        private void RotateY(float angle)
        {
            Vector3 translationVector = currentMatrix.TranslationVector;
            currentMatrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            var a = angle * Math.PI / 180;
            currentMatrix *= Matrix.RotationAxis(currentMatrix.Up, (float)a);
            currentMatrix *= Matrix.Translation(translationVector);
        }

        private void RotateX(float angle)
        {
            Vector3 translationVector = currentMatrix.TranslationVector;
            currentMatrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            var a = angle * Math.PI / 180;
            currentMatrix *= Matrix.RotationAxis(currentMatrix.Right, (float)a);
            currentMatrix *= Matrix.Translation(translationVector);
        }

        private void PushMatrix()
        {
            var matrix = currentMatrix;
            radius -= RadiusReduction;
            if (radius < 0.1f) radius = 0.1f;
            matrices.Push(matrix);
        }

        private void PopMatrix()
        {
            currentMatrix = matrices.Pop();
            radius += RadiusReduction;
        }
    }

    [Serializable]
    public class SaveTreeMemento : IMemento<Tree>
    {
        public string Current = "";
        public float Angle = 1.0f;
        public float Radius = 1.0f;
        public float RadiusReduction = 1.0f;
        public float Length = 1.0f;
        public RuleSet RuleSet;

        public SaveTreeMemento(Tree target)
        {
            Current = target.GetCurrent();
            Angle = target.Angle;
            Length = target.Length;
            Radius = target.Radius;
            RadiusReduction = target.RadiusReduction;
            RuleSet = target.GetRuleSet();
        }

        public IMemento<Tree> Restore(Tree target)
        {
            IMemento<Tree> inverse = new SaveTreeMemento(target);
            target.LoadProject(this);
            target.Generate(false);
            return inverse;
        }
    }
}
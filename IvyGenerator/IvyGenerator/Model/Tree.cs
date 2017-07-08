using System;
using System.Collections.Generic;
using System.Linq;
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
            treeGeom.AddCylinder(currentMatrix.TranslationVector, currentMatrix.TranslationVector + currentMatrix.Forward * length, radius, 12, true, true);
            //treeGeom.AddQuad(new Vector3(1, 1, 1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1));
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
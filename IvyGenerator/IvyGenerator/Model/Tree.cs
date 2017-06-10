using System;
using System.Collections.Generic;
using System.Windows.Markup.Localizer;
using GalaSoft.MvvmLight;
using IvyGenerator.Utilities;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using System.Diagnostics;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace IvyGenerator.Model
{
    public class Tree : ObservableObject
    {
        private LSystem lSys = null;

        private float length = 5.0f;
        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                Generate();
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
                Generate();
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
                Generate();
                RaisePropertyChanged("RadiusReduction");
            }
        }
        private float angle = 25;
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                RaisePropertyChanged("Angle");
            }
        }

        public void SetRuleSet(RuleSet ruleSet)
        {
            lSys = new LSystem(ruleSet);
        }

        private Stack<Matrix> matrices = new Stack<Matrix>();
        LineBuilder lineBuilder = new LineBuilder();
        private MeshGeometry3D treeGeom = new MeshGeometry3D();
        public MeshGeometry3D TreeGeometry { get { return treeGeom; } }

        private Matrix currentMatrix = Matrix.Identity;

        public Tree()
        {
            //Rotate by 90 to face up
            currentMatrix *= Matrix.RotationAxis(Vector3.Right, (float)(90 * Math.PI / 180));
            lineBuilder.AddLine(currentMatrix.TranslationVector, currentMatrix.TranslationVector + currentMatrix.Forward * length);
            treeGeom = new MeshGeometry3D();
            for (int i = 0; i < 6; i++)
            {
                Vector3 pos = Quaternion.RotationAxis(currentMatrix.Forward, 60) * currentMatrix.Right;
            }
        }

        public void Generate()
        {
            lSys.Generate();
            lineBuilder = new LineBuilder();
            currentMatrix = Matrix.RotationAxis(Vector3.Right, (float)(90 * Math.PI / 180));
            string curr = lSys.Current;

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
        }

        private void Line(float len)
        {
            lineBuilder.AddLine(currentMatrix.TranslationVector, currentMatrix.TranslationVector + currentMatrix.Forward * len );
        }

        private void Translate(float len)
        {
            currentMatrix *= Matrix.Translation(currentMatrix.Forward * len);
        }

        private void RotateZ(float angle)
        {
            //Translate to origin
            Vector3 translationVector = currentMatrix.TranslationVector;
            currentMatrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            //Rotate
            var a = angle * Math.PI / 180;
            currentMatrix *= Matrix.RotationAxis(currentMatrix.Forward, (float)a);
            //Translate back
            currentMatrix *= Matrix.Translation(translationVector);
        }

        private void RotateY(float angle)
        {
            //Translate to origin
            Vector3 translationVector = currentMatrix.TranslationVector;
            currentMatrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            //Rotate
            var a = angle * Math.PI / 180;
            currentMatrix *= Matrix.RotationAxis(currentMatrix.Up, (float)a);
            //Translate back
            currentMatrix *= Matrix.Translation(translationVector);
        }

        private void RotateX(float angle)
        {
            //Translate to origin
            Vector3 translationVector = currentMatrix.TranslationVector;
            currentMatrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            //Rotate
            var a = angle * Math.PI / 180;
            currentMatrix *= Matrix.RotationAxis(currentMatrix.Right, (float)a);
            //Translate back
            currentMatrix *= Matrix.Translation(translationVector);
        }

        private void PushMatrix()
        {
            //Create Matrix from current position, add to list.
            var matrix = currentMatrix;
            matrices.Push(matrix);
        }

        private void PopMatrix()
        {
            //Pop off last matrix, reset current position to this.
            currentMatrix = matrices.Pop();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Markup.Localizer;
using IvyGenerator.Utilities;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;

namespace IvyGenerator.Model
{
    public class Tree
    {
        private LSystem lSys = new LSystem();

        private float len = 5.0f;
        private float radius = 0.1f;
        private float radiusReduction = 0.1f;
        private float angle = 25;
        
        private Stack<Matrix> matrices = new Stack<Matrix>();
        LineBuilder lineBuilder = new LineBuilder();
        public LineGeometry3D TreeGeometry { get { return lineBuilder.ToLineGeometry3D(); } }

        private Matrix currentMatrix = Matrix.Identity;

        public Tree()
        {
            //Rotate by 90 to face up
            currentMatrix *= Matrix.RotationAxis(Vector3.Right, (float)(90 * Math.PI / 180));
            lineBuilder.AddLine(currentMatrix.TranslationVector, currentMatrix.TranslationVector + currentMatrix.Forward * len);
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
                        Line(len);
                        Translate(len);
                        break;
                    case 'g'://Move forward
                        Translate(len);
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

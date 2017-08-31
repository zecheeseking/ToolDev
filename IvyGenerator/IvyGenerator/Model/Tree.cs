using GalaSoft.MvvmLight;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core;
using IvyGenerator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Direct2D1.Effects;
using SharpDX.DXGI;
using MeshBuilder = HelixToolkit.Wpf.SharpDX.MeshBuilder;

namespace IvyGenerator.Model
{
    public class Tree : ObservableObject
    {
        private LSystem lSys = null;

        public PhongMaterial BarkMaterial { get; private set; }
        private PhongMaterial leafMaterial;

        public PhongMaterial LeafMaterial
        {
            get { return leafMaterial; }
            set
            {
                leafMaterial = value;
                RaisePropertyChanged("LeafMaterial");
            }
        }

        private float length = 2.0f;
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
        private float radius = 1.0f;
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

        private int treeLimbSides = 6;
        public int TreeLimbSides
        {
            get { return treeLimbSides; }
            set
            {
                treeLimbSides = value;
                Generate(false);
                RaisePropertyChanged("TreeLimbSides");
            }
        }

        private int branchInterpolationPoints = 10;
        public int BranchInterpolationPoints
        {
            get { return branchInterpolationPoints; }
            set
            {
                branchInterpolationPoints = value;
                Generate(false);
                RaisePropertyChanged("BranchInterpolationPoints");
            }
        }

        private int minLeaves = 1;
        public int MinLeaves
        {
            get { return minLeaves; }
            set
            {
                minLeaves = value;
                Generate(false);
                RaisePropertyChanged("MinLeaves");
            }
        }

        private int maxLeaves = 10;
        public int MaxLeaves
        {
            get { return maxLeaves; }
            set
            {
                maxLeaves = value;
                Generate(false);
                RaisePropertyChanged("MaxLeaves");
            }
        }

        private float minLeafScale = 0.9f;
        public float MinLeafScale
        {
            get { return minLeafScale; }
            set
            {
                minLeafScale = value;
                Generate(false);
                RaisePropertyChanged("MinLeafScale");
            }
        }

        private float maxLeafScale = 1.1f;
        public float MaxLeafScale
        {
            get { return maxLeafScale; }
            set
            {
                maxLeafScale = value;
                Generate(false);
                RaisePropertyChanged("MaxLeafScale");
            }
        }

        public void LoadProject(SaveTreeMemento loadedData)
        {
            Angle = loadedData.Angle;
            Radius = loadedData.Radius;
            RadiusReduction = loadedData.RadiusReduction;
            Length = loadedData.Length;
            TreeLimbSides = loadedData.TreeLimbSides;
            BranchInterpolationPoints = loadedData.BranchInterpolationPoints;
            MinLeaves = loadedData.MinLeaves;
            MaxLeaves = loadedData.MaxLeaves;
            MinLeafScale = loadedData.MinLeafScale;
            MaxLeafScale = loadedData.MaxLeafScale;
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

        private struct TreePoint
        {
            public Matrix matrix;
            public Matrix previousBranchEnd;
            public float radius;

            public TreePoint(float radius)
            {
                //matrix = Matrix.Identity;
                matrix = Matrix.RotationAxis(Vector3.Right, (float)(90 * Math.PI / 180));
                previousBranchEnd = matrix;
                this.radius = radius;
            }

            public TreePoint(Matrix matrix, float radius)
            {
                this.matrix = matrix;
                previousBranchEnd = matrix;
                this.radius = radius;
            }
        }

        private Stack<TreePoint> matrices = new Stack<TreePoint>();

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

        private MeshBuilder leavesGeom = new MeshBuilder();
        private MeshGeometry3D leavesGeometry;
        public MeshGeometry3D LeavesGeometry
        {
            get { return leavesGeometry; }
            set
            {
                leavesGeometry = value;
                RaisePropertyChanged("LeavesGeometry");
            }
        }

        private TreePoint currentTreePoint = new TreePoint();

        public Tree()
        {
            //Rotate by 90 to face up
            TreeGeometry = null;
            LeavesGeometry = null;

            var barkImage = Helper.LoadFileToMemory(new Uri(@"TileableBark.jpg", UriKind.RelativeOrAbsolute).ToString());
            BarkMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                DiffuseMap = barkImage
            };

            var image = Helper.LoadFileToMemory(new Uri(@"DefaultLeaf.png", UriKind.RelativeOrAbsolute).ToString());
            LeafMaterial = new PhongMaterial
            {
                AmbientColor = Color.Gray,
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                DiffuseAlphaMap = image,
                DiffuseMap = image
            };

            currentTreePoint = new TreePoint(radius);
        }

        public void Generate(bool advanceGeneration = true)
        {
            if(advanceGeneration)
                lSys.Generate();
            currentTreePoint = new TreePoint(radius);

            string curr = lSys.Current;
            treeGeom = new MeshBuilder();
            leavesGeom = new MeshBuilder();

            foreach (char c in curr)
            {
                switch(c)
                {
                    case 'f'://Create branch
                        Branch(length);
                        Translate(length);
                        break;
                    case 'g'://Move forward
                        Translate(length);
                        break;
                    case 'l':
                        CreateLeaf();
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
            CreateLeaf();
            var LeavesMesh = leavesGeom.ToMeshGeometry3D();
            LeavesMesh.Colors = new Color4Collection(leavesGeom.TextureCoordinates.Select(x => x.ToColor4()));
            LeavesGeometry = LeavesMesh;
        }

        public void Reset()
        {
            lSys.Reset();
            treeGeom = new MeshBuilder();
            TreeGeometry = null;

            leavesGeom = new MeshBuilder();
            LeavesGeometry = null;
        }

        private void Branch(float len)
        {
            SharpDX.Vector3 ps = currentTreePoint.previousBranchEnd.TranslationVector;
            SharpDX.Vector3 pe = currentTreePoint.matrix.TranslationVector + currentTreePoint.matrix.Forward * length;

            ////Spline control points
            float distance = Vector3.Distance(ps, pe);
            Vector3 p1 = ps + currentTreePoint.previousBranchEnd.Forward * (distance / 4);
            Vector3 p2 = pe + currentTreePoint.matrix.Backward * (distance / 5);
            //Spline control points
            //SPLINE CODE
            Vector3 norm = Vector3.Zero;
            Vector3 binorm = Vector3.Zero;

            List<Matrix> splineOrientedPoints = new List<Matrix>();
            for (int i = 0; i <= branchInterpolationPoints; i++)
            {
                float t = i * (1.0f / branchInterpolationPoints);
                float omt = 1f - t;
                float omt2 = omt * omt;
                float t2 = t * t;

                Vector3 pt = ps * (omt2 * omt) +
                             p1 * (3f * omt2 * t) +
                             p2 * (3f * omt * t2)  +
                             pe * (t2 * t);

                Vector3 tan = ps * (-omt2) + 
                    p1 * (3 * omt2 - 2 * omt) + 
                    p2 * (-3 * t2 + 2 * t) +
                    pe * (t2);

                tan = tan.Normalized();
                if (i == 0)
                {
                    Vector3 V = Vector3.Right;
                    norm = Vector3.Cross(tan, V);
                    norm = norm.Normalized();
                    binorm = Vector3.Cross(tan, norm);
                    binorm = binorm.Normalized();
                }
                else
                {
                    norm = Vector3.Cross(binorm, tan);
                    norm = norm.Normalized();
                    binorm = Vector3.Cross(tan, norm);
                    binorm = binorm.Normalized();
                }

                Matrix transform = Matrix.Identity;
                transform *= Matrix.RotationQuaternion(Quaternion.LookAtLH(pt, pt + tan, Vector3.ForwardLH));
                //transform *= Matrix.RotationQuaternion(Quaternion.RotationLookAtRH(pt + tan, norm));
                transform *= Matrix.Translation(pt);
                splineOrientedPoints.Add(transform);
            }

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();
            float shapeAngle = 360.0f / treeLimbSides;

            for (int i = 0; i <= treeLimbSides; i++)
            {
                Vector3 dir = Vector3.TransformCoordinate(splineOrientedPoints[0].Forward, Matrix.RotationAxis(splineOrientedPoints[0].Right, MathUtil.DegreesToRadians(-90)));
                dir.Normalize();
                Matrix rot = Matrix.RotationAxis(splineOrientedPoints[0].Forward, MathUtil.DegreesToRadians(shapeAngle * i));
                dir = Vector3.TransformNormal(dir, rot).Normalized();
                Vector3 v = splineOrientedPoints[0].TranslationVector + dir * currentTreePoint.radius;
                positions.Add(v);
                norms.Add((v - dir).Normalized());
                uvs.Add(new Vector2(1.0f / treeLimbSides * i, 1.0f));
            }

            for (int p = 1; p < splineOrientedPoints.Count; p++)
            {
                float r = MathUtil.Lerp(currentTreePoint.radius, currentTreePoint.radius - radiusReduction, p * (1.0f / branchInterpolationPoints));
                r = MathUtil.Clamp(r, 0.01f, currentTreePoint.radius);
                for (int i = 0; i <= treeLimbSides; i++)
                {
                    Vector3 dir = Vector3.TransformCoordinate(splineOrientedPoints[p].Forward, Matrix.RotationAxis(splineOrientedPoints[p].Right, MathUtil.DegreesToRadians(-90)));
                    dir.Normalize();
                    Matrix rot = Matrix.RotationAxis(splineOrientedPoints[p].Forward, MathUtil.DegreesToRadians(shapeAngle * i));
                    dir = Vector3.TransformNormal(dir, rot).Normalized();
                    Vector3 v = splineOrientedPoints[p].TranslationVector + dir * r;
                    positions.Add(v);
                    norms.Add((v - dir).Normalized());
                    uvs.Add(new Vector2(1.0f / treeLimbSides * i, 1.0f - (1.0f / (splineOrientedPoints.Count - 1) * p)));
                }

                int offset = p == 1 ? 0 : (p - 1) * (treeLimbSides + 1) - 1;
                for (int i = 0; i <= treeLimbSides; i++)
                {
                    int a = i + offset;
                    int b = i + offset + 1;
                    int c = i + offset + treeLimbSides + 1;
                    int d = i + offset + treeLimbSides + 2;

                    indices.Add(a);
                    indices.Add(c);
                    indices.Add(b);
                    indices.Add(b);
                    indices.Add(c);
                    indices.Add(d);
                }
            }
            treeGeom.Append(positions, indices, norms, uvs);

            currentTreePoint.radius -= radiusReduction;
            currentTreePoint.radius = MathUtil.Clamp(currentTreePoint.radius, 0.01f, 100.0f);
        }

        private void Translate(float len)
        {
            currentTreePoint.matrix *= Matrix.Translation(currentTreePoint.matrix.Forward * len);
            currentTreePoint.previousBranchEnd = currentTreePoint.matrix;
        }

        private void CreateLeaf()
        {
            
            int amountOfLeaves = Helper.RandomNumber(minLeaves, maxLeaves);

            for (int i = 0; i < amountOfLeaves; i++)
            {
                //Var calculate random direction from currentTreePoint to go in.
                //We know the point, we just need a random rotation
                Quaternion randomQ =
                    Quaternion.RotationAxis(Vector3.Up, MathUtil.DegreesToRadians(Helper.RandomFloatNumber(0.0f, 360.0f))) *
                    Quaternion.RotationAxis(Vector3.ForwardLH, MathUtil.DegreesToRadians(Helper.RandomFloatNumber(0.0f, 360.0f))) *
                    Quaternion.RotationAxis(Vector3.Right, MathUtil.DegreesToRadians(Helper.RandomFloatNumber(0.0f, 360.0f)));

                Matrix leafTransform = currentTreePoint.matrix * Matrix.RotationQuaternion(randomQ);

                List<Vector3> positions = new List<Vector3>();
                List<Vector3> norms = new List<Vector3>();
                List<Vector2> uvs = new List<Vector2>();
                List<int> indices = new List<int>();

                float LeafScale = Helper.RandomFloatNumber(minLeafScale, maxLeafScale);

                positions.Add(currentTreePoint.matrix.TranslationVector + leafTransform.Right * -(LeafScale / 2));
                positions.Add(currentTreePoint.matrix.TranslationVector + leafTransform.Right * -(LeafScale / 2) + leafTransform.Forward * LeafScale);
                positions.Add(currentTreePoint.matrix.TranslationVector + leafTransform.Right * (LeafScale / 2));
                positions.Add(currentTreePoint.matrix.TranslationVector + leafTransform.Right * (LeafScale / 2) + leafTransform.Forward * LeafScale);

                norms.Add(currentTreePoint.matrix.Forward);
                norms.Add(currentTreePoint.matrix.Forward);
                norms.Add(currentTreePoint.matrix.Forward);
                norms.Add(currentTreePoint.matrix.Forward);

                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));

                indices.Add(0);
                indices.Add(1);
                indices.Add(2);
                indices.Add(1);
                indices.Add(3);
                indices.Add(2);

                leavesGeom.Append(positions, indices, norms, uvs);
            }
        }

        private void RotateZ(float angle)
        {
            Vector3 translationVector = currentTreePoint.matrix.TranslationVector;
            currentTreePoint.matrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            var a = angle * Math.PI / 180;
            currentTreePoint.matrix *= Matrix.RotationAxis(currentTreePoint.matrix.Forward, (float)a);
            currentTreePoint.matrix *= Matrix.Translation(translationVector);
        }

        private void RotateY(float angle)
        {
            Vector3 translationVector = currentTreePoint.matrix.TranslationVector;
            currentTreePoint.matrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            var a = angle * Math.PI / 180;
            currentTreePoint.matrix *= Matrix.RotationAxis(currentTreePoint.matrix.Up, (float)a);
            currentTreePoint.matrix *= Matrix.Translation(translationVector);
        }

        private void RotateX(float angle)
        {
            Vector3 translationVector = currentTreePoint.matrix.TranslationVector;
            currentTreePoint.matrix *= (Matrix.Identity * Matrix.Translation(-translationVector));
            var a = angle * Math.PI / 180;
            currentTreePoint.matrix *= Matrix.RotationAxis(currentTreePoint.matrix.Right, (float)a);
            currentTreePoint.matrix *= Matrix.Translation(translationVector);
        }

        private void PushMatrix()
        {
            matrices.Push(currentTreePoint);
        }

        private void PopMatrix()
        {
            currentTreePoint = matrices.Pop();
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
        public int TreeLimbSides = 6;
        public int BranchInterpolationPoints = 10;
        public int MinLeaves = 1;
        public int MaxLeaves = 10;
        public float MinLeafScale = 1.0f;
        public float MaxLeafScale = 1.0f;
        public RuleSet RuleSet;

        public SaveTreeMemento(Tree target)
        {
            Current = target.GetCurrent();
            Angle = target.Angle;
            Length = target.Length;
            Radius = target.Radius;
            RadiusReduction = target.RadiusReduction;
            TreeLimbSides = target.TreeLimbSides;
            BranchInterpolationPoints = target.BranchInterpolationPoints;
            MinLeaves = target.MinLeaves;
            MaxLeaves = target.MaxLeaves;
            MinLeafScale = target.MinLeafScale;
            MaxLeafScale = target.MaxLeafScale;
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
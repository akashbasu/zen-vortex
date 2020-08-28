using UnityEditor;
using UnityEngine;

namespace RollyVortex.Editor
{
    public static class Tube
    {
        private const uint Sides = 72;
        private const float Height = 1f;

        private const float HeightToWidthMultiplier = 0.5f;
        private const float TubeWallWidth = 0.1f;

        private static readonly float OuterRadius = Height * HeightToWidthMultiplier; //.5f;
        private static readonly float InnerRadius = OuterRadius * 0.25f;

        public static void Make(GameObject gameObject)
        {
            var filter = gameObject.GetComponent<MeshFilter>();
            var mesh = filter.sharedMesh;

            mesh.Clear();

            // Outter shell is at radius1 + radius2 / 2, inner shell at radius1 - radius2 / 2


            var nbVerticesCap = Sides * 2 + 2;
            var nbVerticesSides = Sides * 2 + 2;

            #region Vertices

            // bottom + top + sides
            var vertices = new Vector3[nbVerticesCap * 2 + nbVerticesSides * 2];
            var vert = 0;
            var _2pi = Mathf.PI * 2f;

            // Bottom cap
            var sideCounter = 0;
            while (vert < nbVerticesCap)
            {
                sideCounter = sideCounter == Sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / Sides * _2pi;
                var cos = Mathf.Cos(r1);
                var sin = Mathf.Sin(r1);
                vertices[vert] = new Vector3(cos * (OuterRadius - InnerRadius * .5f), 0f,
                    sin * (OuterRadius - InnerRadius * .5f));
                vertices[vert + 1] = new Vector3(cos * (OuterRadius + InnerRadius * .5f), 0f,
                    sin * (OuterRadius + InnerRadius * .5f));
                vert += 2;
            }

            // Top cap
            sideCounter = 0;
            while (vert < nbVerticesCap * 2)
            {
                sideCounter = sideCounter == Sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / Sides * _2pi;
                var cos = Mathf.Cos(r1);
                var sin = Mathf.Sin(r1);
                vertices[vert] = new Vector3(cos * (OuterRadius - InnerRadius * .5f), Height,
                    sin * (OuterRadius - InnerRadius * .5f));
                vertices[vert + 1] = new Vector3(cos * (OuterRadius + InnerRadius * .5f), Height,
                    sin * (OuterRadius + InnerRadius * .5f));
                vert += 2;
            }

            // Sides (out)
            sideCounter = 0;
            while (vert < nbVerticesCap * 2 + nbVerticesSides)
            {
                sideCounter = sideCounter == Sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / Sides * _2pi;
                var cos = Mathf.Cos(r1);
                var sin = Mathf.Sin(r1);

                vertices[vert] = new Vector3(cos * (OuterRadius + InnerRadius * .5f), Height,
                    sin * (OuterRadius + InnerRadius * .5f));
                vertices[vert + 1] = new Vector3(cos * (OuterRadius + InnerRadius * .5f), 0,
                    sin * (OuterRadius + InnerRadius * .5f));
                vert += 2;
            }

            // Sides (in)
            sideCounter = 0;
            while (vert < vertices.Length)
            {
                sideCounter = sideCounter == Sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / Sides * _2pi;
                var cos = Mathf.Cos(r1);
                var sin = Mathf.Sin(r1);

                vertices[vert] = new Vector3(cos * (OuterRadius - InnerRadius * .5f), Height,
                    sin * (OuterRadius - InnerRadius * .5f));
                vertices[vert + 1] = new Vector3(cos * (OuterRadius - InnerRadius * .5f), 0,
                    sin * (OuterRadius - InnerRadius * .5f));
                vert += 2;
            }

            #endregion

            #region Normales

            // bottom + top + sides
            var normales = new Vector3[vertices.Length];
            vert = 0;

            // Bottom cap
            while (vert < nbVerticesCap) normales[vert++] = Vector3.down;

            // Top cap
            while (vert < nbVerticesCap * 2) normales[vert++] = Vector3.up;

            // Sides (out)
            sideCounter = 0;
            while (vert < nbVerticesCap * 2 + nbVerticesSides)
            {
                sideCounter = sideCounter == Sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / Sides * _2pi;

                normales[vert] = new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
                normales[vert + 1] = normales[vert];
                vert += 2;
            }

            // Sides (in)
            sideCounter = 0;
            while (vert < vertices.Length)
            {
                sideCounter = sideCounter == Sides ? 0 : sideCounter;

                var r1 = (float) sideCounter++ / Sides * _2pi;

                normales[vert] = -new Vector3(Mathf.Cos(r1), 0f, Mathf.Sin(r1));
                normales[vert + 1] = normales[vert];
                vert += 2;
            }

            #endregion

            #region UVs

            var uvs = new Vector2[vertices.Length];

            vert = 0;
            // Bottom cap
            sideCounter = 0;
            while (vert < nbVerticesCap)
            {
                var t = (float) sideCounter++ / Sides;
                uvs[vert++] = new Vector2(0f, t);
                uvs[vert++] = new Vector2(1f, t);
            }

            // Top cap
            sideCounter = 0;
            while (vert < nbVerticesCap * 2)
            {
                var t = (float) sideCounter++ / Sides;
                uvs[vert++] = new Vector2(0f, t);
                uvs[vert++] = new Vector2(1f, t);
            }

            // Sides (out)
            sideCounter = 0;
            while (vert < nbVerticesCap * 2 + nbVerticesSides)
            {
                var t = (float) sideCounter++ / Sides;
                uvs[vert++] = new Vector2(t, 0f);
                uvs[vert++] = new Vector2(t, 1f);
            }

            // Sides (in)
            sideCounter = 0;
            while (vert < vertices.Length)
            {
                var t = (float) sideCounter++ / Sides;
                uvs[vert++] = new Vector2(t, 0f);
                uvs[vert++] = new Vector2(t, 1f);
            }

            #endregion

            #region Triangles

            var nbFace = Sides * 4;
            var nbTriangles = nbFace * 2;
            var nbIndexes = nbTriangles * 3;
            var triangles = new int[nbIndexes];

            // Bottom cap
            var i = 0;
            sideCounter = 0;
            while (sideCounter < Sides)
            {
                var current = sideCounter * 2;
                var next = sideCounter * 2 + 2;

                triangles[i++] = next + 1;
                triangles[i++] = next;
                triangles[i++] = current;

                triangles[i++] = current + 1;
                triangles[i++] = next + 1;
                triangles[i++] = current;

                sideCounter++;
            }

            // Top cap
            while (sideCounter < Sides * 2)
            {
                var current = sideCounter * 2 + 2;
                var next = sideCounter * 2 + 4;

                triangles[i++] = current;
                triangles[i++] = next;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = current + 1;

                sideCounter++;
            }

            // Sides (out)
            while (sideCounter < Sides * 3)
            {
                var current = sideCounter * 2 + 4;
                var next = sideCounter * 2 + 6;

                triangles[i++] = current;
                triangles[i++] = next;
                triangles[i++] = next + 1;

                triangles[i++] = current;
                triangles[i++] = next + 1;
                triangles[i++] = current + 1;

                sideCounter++;
            }


            // Sides (in)
            while (sideCounter < Sides * 4)
            {
                var current = sideCounter * 2 + 6;
                var next = sideCounter * 2 + 8;

                triangles[i++] = next + 1;
                triangles[i++] = next;
                triangles[i++] = current;

                triangles[i++] = current + 1;
                triangles[i++] = next + 1;
                triangles[i++] = current;

                sideCounter++;
            }

            #endregion

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.Optimize();

            filter.sharedMesh = mesh;
            EditorUtility.SetDirty(gameObject);
        }
    }
}
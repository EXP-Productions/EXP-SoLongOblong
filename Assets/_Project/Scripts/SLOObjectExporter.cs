using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    // Takes an SLO object and exports it
    public class SLOObjectExporter : MonoBehaviour
    {
        // File output
        public bool m_WriteFile = false;
        public string fileName = "MyFile.txt";

        public TextMesh m_TextMeshPrefab;

        // Uber hax, fix with maths later
        void LayFacesFlat()
        {
            GameObject parent = new GameObject(m_MeshParent.name + " Flat Faces ");
            parent.transform.position = Vector3.right * 5;
            TriMesh[] flatFaces = new TriMesh[m_Faces.Count];
            float xPos = 7;
            float yPos = 0;
            float prevTriDist = 0;
            float largestTri = 0;

            // For all faces
            for (int i = 0; i < m_Faces.Count; i++)
            {
                // Create a new trimesh
                TriMesh newTriMesh = new GameObject(m_MeshParent.name + " - Flat face " + i).AddComponent<TriMesh>();
                newTriMesh.gameObject.layer = 8;
                Vector3 midPoint = (m_Faces[i].Join0.transform.position + m_Faces[i].Join1.transform.position + m_Faces[i].Join2.transform.position) / 3f;

                newTriMesh.CreateTri(
                    newTriMesh.transform.TransformPoint(m_Faces[i].Join0.transform.position - midPoint),
                    newTriMesh.transform.TransformPoint(m_Faces[i].Join1.transform.position - midPoint),
                    newTriMesh.transform.TransformPoint(m_Faces[i].Join2.transform.position - midPoint),
                    TriMesh.Extrusion.None);

                // Apply material
                newTriMesh.GetComponent<MeshRenderer>().material = m_FlatFaceMat;

                #region Spacing and placement
                // Get max vertex distance on x to make sure next tri is spaced accordingly
                float maxVertDistance = 0;
                for (int j = 0; j < 3; j++)
                {
                    float dist = Vector3.Distance(newTriMesh.transform.position, newTriMesh.transform.TransformPoint(newTriMesh.BaseVerts[j]));
                    if (dist > maxVertDistance)
                        maxVertDistance = dist;
                }
                xPos += maxVertDistance + prevTriDist;

                prevTriDist = maxVertDistance;

                newTriMesh.transform.rotation = Quaternion.FromToRotation(m_Faces[i].Normal, -Vector3.forward);

                if (maxVertDistance > largestTri)
                    largestTri = maxVertDistance;

                if (i == m_Faces.Count / 3)
                {
                    xPos = 7;
                    yPos = largestTri * 2f;
                }
                else if (i == (int)(m_Faces.Count / 3) * 2)
                {
                    xPos = 7;
                    yPos = largestTri * 4f;
                }

                Vector3 offset = new Vector3(xPos, yPos, -1);

                newTriMesh.transform.position = offset;
                newTriMesh.transform.parent = parent.transform;
                #endregion

                TextMesh edgeLength0 = Instantiate(m_TextMesh);
                edgeLength0.name = "Edge text 01";
                edgeLength0.gameObject.layer = 8;
                edgeLength0.transform.SetParent(newTriMesh.transform);
                edgeLength0.transform.position = newTriMesh.Edge01MidPoint;
                edgeLength0.transform.localScale = Vector3.one * .02f;
                edgeLength0.text = m_Faces[i].Edge2Length.ToString("##");

                TextMesh edgeLength1 = Instantiate(m_TextMesh);
                edgeLength1.name = "Edge text 12";
                edgeLength1.gameObject.layer = 8;
                edgeLength1.transform.SetParent(newTriMesh.transform);
                edgeLength1.transform.position = newTriMesh.Edge12MidPoint;
                edgeLength1.transform.localScale = Vector3.one * .02f;
                edgeLength1.text = m_Faces[i].Edge1Length.ToString("##");

                TextMesh edgeLength2 = Instantiate(m_TextMesh);
                edgeLength2.name = "Edge text 20";
                edgeLength2.gameObject.layer = 8;
                edgeLength2.transform.SetParent(newTriMesh.transform);
                edgeLength2.transform.position = newTriMesh.Edge20MidPoint;
                edgeLength2.transform.localScale = Vector3.one * .02f;
                edgeLength2.text = m_Faces[i].Edge0Length.ToString("##");



                flatFaces[i] = newTriMesh;

                //newTriMesh.transform.LookAt( Vector3.up );
                //SLO_Face newFace = Instantiate( m_Faces[i] );
                //newFace.transform.position = 
            }

            //CombineMeshes (parent);
        }
    }
}

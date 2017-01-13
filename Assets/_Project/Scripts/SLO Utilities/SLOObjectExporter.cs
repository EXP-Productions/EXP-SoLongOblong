using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoLongOblong
{
    // Takes an SLO object and exports it
    public class SLOObjectExporter : MonoBehaviour
    {
        public string m_SavePath;
        public string m_ProjectName = "Project 01";

        // File output
        public bool m_WriteFile = false;
        public string fileName = "MyFile.txt";

        public TextMesh m_TextMeshPrefab;

        // Uber hax, fix with maths later
        void LayFacesFlat( SLOObject sloObject )
        {
            // reference to faces from the object
            List<SLO_Face> faces = sloObject.Faces;

            GameObject parent = new GameObject("_Flat faces parent");
            
            parent.transform.position = Vector3.right * 5;
            TriMesh[] flatFaces = new TriMesh[faces.Count];
            float xPos = 7;
            float yPos = 0;
            float prevTriDist = 0;
            float largestTri = 0;

            // For all faces
            for (int i = 0; i < faces.Count; i++)
            {
                // Create a new trimesh
                TriMesh newTriMesh = new GameObject(parent.name + " - Flat face " + i).AddComponent<TriMesh>();
                newTriMesh.gameObject.layer = 8;
                Vector3 midPoint = (faces[i].Join0.transform.position + faces[i].Join1.transform.position + faces[i].Join2.transform.position) / 3f;

                newTriMesh.CreateTri(
                    newTriMesh.transform.TransformPoint(faces[i].Join0.transform.position - midPoint),
                    newTriMesh.transform.TransformPoint(faces[i].Join1.transform.position - midPoint),
                    newTriMesh.transform.TransformPoint(faces[i].Join2.transform.position - midPoint),
                    TriMesh.Extrusion.None);

                // Apply material
                newTriMesh.GetComponent<MeshRenderer>().material = SLOResourceManager.Instance.m_MatFace;

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

                newTriMesh.transform.rotation = Quaternion.FromToRotation(faces[i].Normal, -Vector3.forward);

                if (maxVertDistance > largestTri)
                    largestTri = maxVertDistance;

                if (i == faces.Count / 3)
                {
                    xPos = 7;
                    yPos = largestTri * 2f;
                }
                else if (i == (int)(faces.Count / 3) * 2)
                {
                    xPos = 7;
                    yPos = largestTri * 4f;
                }

                Vector3 offset = new Vector3(xPos, yPos, -1);

                newTriMesh.transform.position = offset;
                newTriMesh.transform.parent = parent.transform;
                #endregion

                TextMesh edgeLength0 = new GameObject("Textmesh").AddComponent<TextMesh>();
                edgeLength0.name = "Edge text v0-v1";
                edgeLength0.gameObject.layer = 8;
                edgeLength0.transform.SetParent(newTriMesh.transform);
                edgeLength0.transform.position = newTriMesh.Edge01MidPoint;
                edgeLength0.transform.localScale = Vector3.one * .02f;
                edgeLength0.text = faces[i].Edge2Length.ToString("##");

                TextMesh edgeLength1 = new GameObject("Textmesh").AddComponent<TextMesh>();
                edgeLength1.name = "Edge text v1-v2";
                edgeLength1.gameObject.layer = 8;
                edgeLength1.transform.SetParent(newTriMesh.transform);
                edgeLength1.transform.position = newTriMesh.Edge12MidPoint;
                edgeLength1.transform.localScale = Vector3.one * .02f;
                edgeLength1.text = faces[i].Edge1Length.ToString("##");

                TextMesh edgeLength2 = new GameObject("Textmesh").AddComponent<TextMesh>();
                edgeLength2.name = "Edge text v2-v0";
                edgeLength2.gameObject.layer = 8;
                edgeLength2.transform.SetParent(newTriMesh.transform);
                edgeLength2.transform.position = newTriMesh.Edge20MidPoint;
                edgeLength2.transform.localScale = Vector3.one * .02f;
                edgeLength2.text = faces[i].Edge0Length.ToString("##");

                flatFaces[i] = newTriMesh;
            }
        }

        public void OutputAssets()
        {
        }
            /*
               public void OutputAssets()
               {
                   // BAKE
                   //TODO: Create copy, bake, output, delete. this way you maintain a working copy

                   copyJoinersParent = new GameObject("Copy joiners");
                   copyJoinersParent.transform.position = m_MeshParent.position;
                   m_JoinerCopies = new SLO_Join[m_Joiners.Count];

                   // parent all tabs
                   for (int j = 0; j < m_Tabs.Count; j++)
                   {
                       m_Tabs[j].ParentToJoin(true);
                   }

                   for (int i = 0; i < m_JoinerCopies.Length; i++)
                   {
                       SLO_Join copyJoin = Instantiate(m_Joiners[i]);
                       copyJoin.enabled = false;
                       copyJoin.name = m_Joiners[i].name;

                       copyJoin.transform.parent = copyJoinersParent.transform;

                       m_JoinerCopies[i] = copyJoin;
                   }

                   // Combine the joint meshes
                   for (int j = 0; j < m_Joiners.Count; j++)
                   {

                       CombineMeshes(m_JoinerCopies[j].gameObject);
                   }

                   // parent all tabs
                   for (int j = 0; j < m_Tabs.Count; j++)
                   {
                       m_Tabs[j].ParentToJoin(false);
                   }


                   // Create folder
                   System.IO.Directory.CreateDirectory(m_SavePath + "/" + m_ProjectName);

                   for (int i = 0; i < m_JoinerCopies.Length; i++)
                   {
                       SLO_Join join = m_JoinerCopies[i];
                       Mesh joinerMesh = join.GetComponent<MeshFilter>().mesh;
                       joinerMesh = MeshExtensions.ScaleVerts(joinerMesh, 1000);
                       //STL.ExportBinary( join.GetComponent< MeshFilter >(), Application.dataPath + join.name + ".STL" );
                       STL.ExportBinary(join.GetComponent<MeshFilter>(), m_SavePath + "/" + m_ProjectName + "/" + m_ProjectName + " - Join " + i + " -  D " + m_EdgeDiameter + "  L " + m_JoinerLength + "  T " + m_JointWallThickness + ".STL");

                       join.GetComponent<MeshFilter>().mesh = MeshExtensions.ScaleVerts(joinerMesh, -1000);
                   }

                   print("Output " + m_JoinerCopies.Length + " STLs");

                   // Export STLs

                   Destroy(copyJoinersParent);

                   WriteFile();
               }

                void WriteFile()
           {

               List< SLO_Edge > sortedEdges = new List< SLO_Edge >();
               sortedEdges = m_Edges.OrderBy (x => x.Length).ToList ();  

               var sr = File.CreateText( m_SavePath + "/" + m_ProjectName + "/" + m_ProjectName + ".txt" );

               CalculateTotalArea();
               ///
               //TODO: group together same edge lengths into multiples

               sr.WriteLine( "CUT LIST \n" + fileName );

               sr.WriteLine("Total area: " + m_TotalArea);

               sr.WriteLine( "" );
               sr.WriteLine( "" );
               sr.WriteLine( "Edges:   Round bar: {0}mm \t  Total length: {1} ", m_EdgeDiameter.ToString("##"), m_TotalEdgeLength * 1000f  );
               sr.WriteLine( "" );
               sr.WriteLine( "Joint length {0}mm", m_JoinerLength.ToString("##") );
               sr.WriteLine( "Length (mm) \t\t Edge number" );
               for (int i = 0; i < sortedEdges.Count; i++) 
               {
                   sr.WriteLine( "\t{0} \t\t {1}",(sortedEdges[i].Length * 1000).ToString("##"), sortedEdges[i].m_Index );
               }

               sr.WriteLine( "\n\n" );

               for (int i = 0; i < m_Edges.Count; i++)
               {
                   sr.WriteLine( "Edge {0}", i );
                   sr.WriteLine("\t Length: {0}  \t\t Vertex 0: {1}   \t\t Vertex 1: {2}", m_Edges[i].Length, m_Edges[i].m_Join0.transform.position, m_Edges[i].m_Join1.transform.position);

                   sr.WriteLine( "\t\t Vertex 0 connects too: " );
                   foreach( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert0 )
                       sr.WriteLine("\t\t\t Edge {0}", edge.m_Index );

                   sr.WriteLine("\t\t  Vertex 1 connects too: ");
                   foreach ( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert1)
                       sr.WriteLine("\t\t\t Edge {0}", edge.m_Index);

                   sr.WriteLine("");
               }

               sr.WriteLine( "TRIS" );
               for (int i = 0; i < m_Faces.Count; i++)
               {
                   //length
                   //angle to next length
                   sr.WriteLine( "Edge {0}", i );
                   sr.WriteLine("\t Length: {0}  \t\t Vertex 0: {1}   \t\t Vertex 1: {2}", m_Edges[i].Length, m_Edges[i].m_Join0.transform.position, m_Edges[i].m_Join1.transform.position);

                   sr.WriteLine( "\t\t Vertex 0 connects too: " );
                   foreach( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert0 )
                       sr.WriteLine("\t\t\t Edge {0}", edge.m_Index );

                   sr.WriteLine("\t\t  Vertex 1 connects too: ");
                   foreach ( SLO_Edge edge in m_Edges[i].EdgesConnectedToVert1)
                       sr.WriteLine("\t\t\t Edge {0}", edge.m_Index);

                   sr.WriteLine("");
               }

               sr.Close();
           }
               */
        }
}

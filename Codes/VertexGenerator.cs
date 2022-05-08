using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VertexGenerator : MonoBehaviour
{
    public GameObject vertexPrefab;
    public GameObject connectionPrefab;

    public Transform vertexParent;
    public Transform connectionParent;

    public int mazeLength;
    public int mazeWidth;

    public bool IDFSResult;

    public List<Vector2> visited;

    public Vector2 Source;
    public Vector2 Target;
    public int depth;
    public List<Vector2> IDFvisited;

    public List<Vector2> USvisited;

    public List<Vector2> IDFPath;
    public List<GameObject> IDFPath2;

    public System.DateTime starttime;
    public System.DateTime endtime;

    public System.TimeSpan FinishTime;

    public Text DepthText;
    public Text TimeText;


    // Start is called before the first frame update
    void Start()
    {
        CreateMaze(mazeLength, mazeWidth);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CreateMaze(int x, int y)
    {
        for(int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                GameObject vertex = Instantiate(vertexPrefab, new Vector3(i, -j, 0), Quaternion.identity, vertexParent);
                vertex.name = i.ToString() +" " + j.ToString();

                if (i - 1 >= 0 )
                {
                    vertex.GetComponent<Cell>().neighbours.Add(new Vector2(i - 1, j));
                }
                if (i + 1 <= x-1)
                {
                    vertex.GetComponent<Cell>().neighbours.Add(new Vector2(i + 1, j ));
                }
                if (j - 1 >= 0)
                {
                    vertex.GetComponent<Cell>().neighbours.Add(new Vector2(i , j - 1));
                }
                if (j + 1 <= y-1)
                {
                    vertex.GetComponent<Cell>().neighbours.Add(new Vector2(i , j + 1));
                }
            }
        }
    }

    public void ConnectCells(Vector2 vertex, Vector2 nextVertex)
    {
        GameObject connection = Instantiate(connectionPrefab, new Vector3((vertex.x + nextVertex.x)/2, -((vertex.y + nextVertex.y) / 2), 1), Quaternion.identity, connectionParent);
        connection.name = vertex.x.ToString() + " " + vertex.y.ToString() + " - " + nextVertex.x.ToString() + " " + nextVertex.y.ToString();


        connection.GetComponent<Connection>().parent1 = vertex;
        connection.GetComponent<Connection>().parent2 = nextVertex;

        GameObject.Find(vertex.x.ToString() + " " + vertex.y.ToString()).GetComponent<Cell>().mazeneighbours.Add(nextVertex);

    }


    public void CreateMaze()
    {
        Vector2 startVertex = new Vector2(0, 0);
        RandomizedDFS(startVertex);
    }
    public void RandomizedDFS(Vector2 vertex)
    {
        visited.Add(vertex);

        shuffle(GameObject.Find(vertex.x.ToString() + " " + vertex.y.ToString()).GetComponent<Cell>().neighbours);

        foreach (var nextVertex in GameObject.Find(vertex.x.ToString() + " " + vertex.y.ToString()).GetComponent<Cell>().neighbours)
        {
            if(visited.Contains(nextVertex))
            {

            }
            else
            {
                ConnectCells(vertex, nextVertex);

                RandomizedDFS(nextVertex);
            }

        }

    }
    public void MarkVisited(Vector2 vertex)
    {
        GameObject.Find(vertex.x.ToString() + " " + vertex.y.ToString()).GetComponent<Cell>().mazeGenerateVisited = true;
    }

    public Cell CellFind(Vector2 vertex)
    {
        return GameObject.Find(vertex.x.ToString() + " " + vertex.y.ToString()).GetComponent<Cell>();

        
    }

    public void shuffle(List<Vector2> list)
    {

        for (int i = 0; i < list.Count; i++)
        {
            Vector2 temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    //              Search Algorithms


    public void callIDFS()
    {
        starttime = System.DateTime.Now;
        IDFSResult = IDDFS(Source, Target, depth);

    }

    public bool IDDFS(Vector2 src, Vector2 target, int depth)
    {
        for(int i = 0; i< depth; i++)
        {
            IDFPath.Clear();
            IDFPath2.Clear();
            //Debug.Log("IDDFS depth is " + depth + " i = " + i);
            IDFvisited = new List<Vector2>();
            if (DLS(src, target, i) == true)
            {
                DepthText.text = "Depth : " + i;
                return true;
            }
        }
        return false;
    }

    public bool DLS(Vector2 src, Vector2 target, int depth)
    {

        IDFPath.Add(src);
        if (src.x == target.x && src.y == target.y)
        {
            Debug.Log("Girdim");
            endtime = System.DateTime.Now;
            System.TimeSpan timeSpan = endtime.Subtract(starttime);
            FinishTime = timeSpan;
            TimeText.text = "Time : " + timeSpan.ToString();
            Debug.Log(timeSpan);
            return true;
        }
        if (depth <= 0)
        {return false;}

        IDFvisited.Add(src);

        foreach (var nextVertex in GameObject.Find(src.x.ToString() + " " + src.y.ToString()).GetComponent<Cell>().mazeneighbours)
        {
            if (IDFvisited.Contains(nextVertex))
            {
            }
            else
            {
                IDFPath.Add(src);
                IDFPath2.Add(GameObject.Find(src.x.ToString() + " " + src.y.ToString() + " - " + nextVertex.x.ToString() + " " + nextVertex.y.ToString()).gameObject);
                if (DLS(nextVertex, target, depth - 1))
                {
                    return true;
                }
            }
        }
            return false;
    }

    public void showPath(string searchtype)
    {
        if(searchtype.Equals("IDFS"))
        {
            for(int i = 0; i< IDFPath.Count;i++)
            {
                GameObject.Find(IDFPath[i].x.ToString() + " " + IDFPath[i].y.ToString()).GetComponent<SpriteRenderer>().color= Color.green;
            }

            for (int i = 0; i < IDFPath2.Count; i++)
            {
                IDFPath2[i].GetComponent<SpriteRenderer>().color = Color.green;
            }

        }
    }

}

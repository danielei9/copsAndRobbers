using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    //GameObjects
    public GameObject board;
    public GameObject[] cops = new GameObject[2];
    public GameObject robber;
    public Text rounds;
    public Text finalMessage;
    public Button playAgainButton;

    //Otras variables
    Tile[] tiles = new Tile[Constants.NumTiles];
    private int roundCount = 0;
    private int state;
    private int clickedTile = -1;
    private int clickedCop = 0;

    void Start()
    {
        InitTiles();
        InitAdjacencyLists();
        state = Constants.Init;
    }

    //Rellenamos el array de casillas y posicionamos las fichas
    void InitTiles()
    {
        for (int fil = 0; fil < Constants.TilesPerRow; fil++)
        {
            GameObject rowchild = board.transform.GetChild(fil).gameObject;

            for (int col = 0; col < Constants.TilesPerRow; col++)
            {
                GameObject tilechild = rowchild.transform.GetChild(col).gameObject;
                tiles[fil * Constants.TilesPerRow + col] = tilechild.GetComponent<Tile>();
            }
        }

        cops[0].GetComponent<CopMove>().currentTile = Constants.InitialCop0;
        cops[1].GetComponent<CopMove>().currentTile = Constants.InitialCop1;
        robber.GetComponent<RobberMove>().currentTile = Constants.InitialRobber;
    }

    public void InitAdjacencyLists()
    {
        //Matriz de adyacencia
        int[,] matriu = new int[Constants.NumTiles, Constants.NumTiles];

        //TODO: Inicializar matriz a 0's        

        for (int i = 0; i < Constants.NumTiles; i++)
        {
            for (int j = 0; j < Constants.NumTiles; j++)
            {
                matriu[i, j] = 0;
            }
        }

        //TODO: Para cada posición, rellenar con 1's las casillas adjList (arriba, abajo, izquierda y derecha)
        for (int i = 0; i < Constants.NumTiles; i++)
        {
            for (int j = 0; j < Constants.NumTiles; j++)
            {
                findAllPossibles(i, j, Constants.NumTiles, Constants.NumTiles);
            }
        }
        

        //TODO: Rellenar la lista "adjacency" de cada casilla con los índices de sus casillas adjList
        for (int i = 0; i < Constants.NumTiles; i++)
        {

            if (i%8 == 0)
            {
                if (i - 8 > 0)
                {
                    tiles[i].adjacency.Add(i - 8);
                }
                if (i + 1 < 63)
                {
                    tiles[i].adjacency.Add(i + 1);
                }
                if (i + 8 < 63)
                {
                    tiles[i].adjacency.Add(i + 8);
                }
            }
            if (i%8 == 7)
            {
                if (i - 1 > 0)
                {
                    tiles[i].adjacency.Add(i - 1);
                }
                if (i - 8 > 0)
                {
                    tiles[i].adjacency.Add(i - 8);
                }
                if (i + 8 < 63)
                {
                    tiles[i].adjacency.Add(i + 8);
                }
            }
            else
            {
                if (i - 1 > 0)
                {
                    tiles[i].adjacency.Add(i - 1);
                }
                if (i - 8 > 0)
                {
                    tiles[i].adjacency.Add(i - 8);
                }
                if (i + 1 < 63)
                {
                    tiles[i].adjacency.Add(i + 1);
                }
                if (i + 8 < 63)
                {
                    tiles[i].adjacency.Add(i + 8);
                }
            }

        }

    }
    public int[,] findAllPossibles(int i, int j, int rows, int columns)
    {
        int[,] matriu = new int[Constants.NumTiles, Constants.NumTiles];
        if (i < rows - 1 && i != 0)
        {
            matriu[i + 1, j] = 1;
            matriu[i - 1, j] = 1;
        }
        if (i < rows - 2 && i >= 2)
        {
            matriu[i + 2, j] = 1;
            matriu[i - 2, j] = 1;
        }
        if (j < rows - 1 && j != 0)
        {
            matriu[i, j + 1] = 1;
            matriu[i, j - 1] = 1;
        }

        if (j < rows - 2 && j >= 2)
        {
            matriu[i, j + 2] = 1;
            matriu[i, j - 2] = 1;
        }
        if (i == 0)
        {
            matriu[i + 1, j] = 1;
            matriu[i + 2, j] = 1;
        }
        if (i == rows - 2)
        {
            matriu[i + 1, j] = 1;
            matriu[i - 1, j] = 1;
            matriu[i - 2, j] = 1;
        }
        if (i == rows - 1)
        {
            matriu[i - 1, j] = 1;
            matriu[i - 2, j] = 1;
        }

        if (j == 0)
        {
            matriu[i, j + 1] = 1;
            matriu[i, j + 2] = 1;
        }
        if (j == columns - 2)
        {
            matriu[i, j + 1] = 1;
            matriu[i, j - 1] = 1;
            matriu[i, j - 2] = 1;
        }
        if (j == columns - 1)
        {
            matriu[i, j - 1] = 1;
            matriu[i, j - 2] = 1;
        }
        return matriu;
    }
    //Reseteamos cada casilla: color, padre, distance y visitada
    public void ResetTiles()
    {
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }

    public void ClickOnCop(int cop_id)
    {
        switch (state)
        {
            case Constants.Init:
            case Constants.CopSelected:
                clickedCop = cop_id;
                clickedTile = cops[cop_id].GetComponent<CopMove>().currentTile;
                tiles[clickedTile].current = true;

                ResetTiles();
                FindSelectableTiles(true);

                state = Constants.CopSelected;
                break;
        }
    }

    public void ClickOnTile(int t)
    {
        clickedTile = t;

        switch (state)
        {
            case Constants.CopSelected:
                //Si es una casilla roja, nos movemos
                if (tiles[clickedTile].selectable)
                {
                    cops[clickedCop].GetComponent<CopMove>().MoveToTile(tiles[clickedTile]);
                    cops[clickedCop].GetComponent<CopMove>().currentTile = tiles[clickedTile].numTile;
                    tiles[clickedTile].current = true;

                    state = Constants.TileSelected;
                }
                break;
            case Constants.TileSelected:
                state = Constants.Init;
                break;
            case Constants.RobberTurn:
                state = Constants.Init;
                break;
        }
    }

    public void FinishTurn()
    {
        switch (state)
        {
            case Constants.TileSelected:
                ResetTiles();

                state = Constants.RobberTurn;
                RobberTurn();
                break;
            case Constants.RobberTurn:
                ResetTiles();
                IncreaseRoundCount();
                if (roundCount <= Constants.MaxRounds)
                    state = Constants.Init;
                else
                    EndGame(false);
                break;
        }
    }

    public void RobberTurn()
    {
        clickedTile = robber.GetComponent<RobberMove>().currentTile;
        tiles[clickedTile].current = true;
        FindSelectableTiles(false);

        /*TODO: Cambia el código de abajo para hacer lo siguiente   
        - Elegimos una casilla aleatoria entre las seleccionables que puede ir el caco
        - Movemos al caco a esa casilla
        - Actualizamos la variable currentTile del caco a la nueva casilla
        */ // 

    }

    public void EndGame(bool end)
    {
        if (end)
        {
            finalMessage.color = Color.green;
            finalMessage.text = "You Win!";
        }
        else
        {
            finalMessage.color = Color.green;
            finalMessage.text = "You Lose!";
        }

        playAgainButton.interactable = true;
        state = Constants.End;
    }

    public void PlayAgain()
    {
        cops[0].GetComponent<CopMove>().Restart(tiles[Constants.InitialCop0]);
        cops[1].GetComponent<CopMove>().Restart(tiles[Constants.InitialCop1]);
        robber.GetComponent<RobberMove>().Restart(tiles[Constants.InitialRobber]);

        ResetTiles();

        playAgainButton.interactable = false;
        finalMessage.text = "";
        roundCount = 0;
        rounds.text = "Rounds: ";

        state = Constants.Restarting;
    }

    public void InitGame()
    {
        state = Constants.Init;
    }

    public void IncreaseRoundCount()
    {
        roundCount++;
        rounds.text = "Rounds: " + roundCount;
    }

    public void FindSelectableTiles(bool cop)
    {
        int indexcurrentTile;
        int cop2 = 2;
        List<int> adjList = new List<int>();
        List<int> tieldRobber = new List<int>();

        // Si es un cop -> player
        if (cop == true)
        {
            //Try to catch the othe cop
            if (clickedCop == 0) cop2 = 1;
            else
                cop2 = 0;
            // Getting Current player tiel
            indexcurrentTile = cops[clickedCop].GetComponent<CopMove>().currentTile;

            // Add Ajacencies
            for (int j = 0; j < tiles[indexcurrentTile].adjacency.Count; j++)
            {
                adjList.Add(tiles[indexcurrentTile].adjacency[j]);
            }

            adjList.Add(indexcurrentTile);
            // convert to selectable Tile
            for (int j = 0; j < adjList.Count; j++)
            {
                for (int i = 0; i < tiles[adjList[j]].adjacency.Count; i++)
                {
                    if (!(cops[cop2].GetComponent<CopMove>().currentTile == adjList[j]))
                    {
                        // NOT CURRENT TILE
                        if (tiles[tiles[adjList[j]].adjacency[i]].numTile != indexcurrentTile)
                        {
                            tiles[tiles[adjList[j]].adjacency[i]].selectable = true;
                            tiles[cops[cop2].GetComponent<CopMove>().currentTile].selectable = false;
                        }
                    }
                }
            }
        } //Finish to add

        else  // Rober Turn
        {
            // Declared
            List<int> distances = new List<int>();
            List<int> axisYRobber = new List<int>();
            List<int> axisXRobber = new List<int>();
            List<int> copPosition = new List<int>();
            List<int> axisY = new List<int>();
            List<int> axisX = new List<int>();
            List<int> distanceTotal = new List<int>();
            int axisYTotal;
            int axisXTotal;
            int farAwayTield;
            int farAwaydistance;
            
            // initialized
            axisYTotal = 0;
            axisXTotal = 0;
            distances.Add(0);
            distances.Add(0);
            farAwaydistance = 0;
            farAwayTield = 0;

            copPosition.Add(cops[0].GetComponent<CopMove>().currentTile);
            copPosition.Add(cops[1].GetComponent<CopMove>().currentTile);
    
            // Add selectables adjList
            indexcurrentTile = robber.GetComponent<RobberMove>().currentTile;

            for (int j = 0; j < tiles[indexcurrentTile].adjacency.Count; j++)
            {
                if (!(tiles[indexcurrentTile].adjacency[j] == copPosition[0] || tiles[indexcurrentTile].adjacency[j] == copPosition[1]))
                {
                    adjList.Add(tiles[indexcurrentTile].adjacency[j]);
                }
            }

            adjList.Add(indexcurrentTile);
            // Making selectables
            for (int j = 0; j < adjList.Count; j++)
            {
                for (int i = 0; i < tiles[adjList[j]].adjacency.Count; i++)
                {
                    if (!(tiles[adjList[j]].adjacency[i] == copPosition[0] || tiles[adjList[j]].adjacency[i] == copPosition[1]))
                    {
                        tiles[tiles[adjList[j]].adjacency[i]].selectable = true;
                        tieldRobber.Add(tiles[adjList[j]].adjacency[i]);
                    }
                }
            }// Finish making selectables

            tieldRobber.Remove(indexcurrentTile);
            // Calculate distance
            for (int j = 0; j < tieldRobber.Count; j++)
            {
                axisXRobber.Add(tieldRobber[j]);
                axisYRobber.Add(0);

                while (axisXRobber[j] >= 0)
                {
                    axisXRobber[j] -= 8;
                    axisYRobber[j]++;
                }

                axisYRobber[j] -= 1;
                axisXRobber[j] += 8;

                for (int i = 0; i < copPosition.Count; i++)
                {
                    //current cop tile Add
                    axisX.Add(copPosition[i]);
                    axisY.Add(0);
                    // from coordenates to index calculates
                    while (axisX[i] >= 0)
                    {  
                        axisX[i] -= 8;
                        axisY[i]++;
                    }
                    axisY[i] -= 1;
                    axisX[i] += 8;

                    axisYTotal = axisYRobber[j] - axisY[i];
                    if (axisYTotal < 0)
                    {
                        axisYTotal = axisYTotal * -1;
                    }

                    axisXTotal = axisXRobber[j] - axisX[i];
                    if (axisXTotal < 0)
                    {
                        axisXTotal = axisXTotal * -1;
                    }

                    distances[i] = axisYTotal + axisXTotal;
                }
                // ADd disntaces
                distanceTotal.Add(distances[0] + distances[1]);
            }

            for (int i = 0; i < distanceTotal.Count; i++)
            {
                // If find other farest take it 
                if (distanceTotal[i] >= farAwaydistance)
                {
                    farAwaydistance = distanceTotal[i];
                    farAwayTield = i;
                }
            }
            // 
            robber.GetComponent<RobberMove>().MoveToTile(tiles[tieldRobber[farAwayTield]]);
            robber.GetComponent<RobberMove>().currentTile = tieldRobber[farAwayTield];
        }

        //reset pink tield
        tiles[indexcurrentTile].current = true;
    }









}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct cell{ //one of the 81 spaces of the board, handles text updates and cell data
    public int num { // requires 1-9 values, 0 for empty
        set{//also update text
            _num = value;

            if (num != 0){ //update the number for non-empty
                text.color = (given) ? Color.black : new Color(0.41f, 0.01f, 0.46f);//color the given values differently from guesses
                text.text = _num.ToString();
            }
            else { //activate contraint display
                // text.text = ""; //for no display for empty
                text.text = _contraintsCount.ToString();
                text.color = Color.red;
            }
        }
        get{
            return _num;
        }
    }
    public int contraintsCount{
        set{
            _contraintsCount = value;

            if (num == 0){//update display
                text.text = _contraintsCount.ToString();
                text.color = Color.red;
            }
        }
        get{
            return _contraintsCount;
        }
    }
    private int _num;
    public int _contraintsCount;//number of non-empties in the same column row and box. The higher this number, the less options for input
    public bool given; // was this a given/clue value in the puzzle (used for text coloring)
    public Text text; //number display of this space in Unity UI
}

public class board : MonoBehaviour
{
    cell[,] cells = new cell[9,9];

    //link to child UI element for mapping UI to code
    //represents a 3x3 with indices in phone order
    // 0 1 2
    // 3 4 5
    // 6 7 8
    [SerializeField] box[] boxes;

    int[,] givenExample = new int[9,9]{//0's are blank spaces
        {2,0,7,0,1,3,0,6,8},
        {0,0,0,6,8,0,3,4,0},
        {8,0,3,0,5,0,0,0,0},
        {3,1,0,4,7,0,0,8,5},
        {0,0,0,1,6,2,0,0,4},
        {6,0,0,0,0,8,2,0,9},
        {0,2,0,0,9,1,0,0,0},
        {4,3,9,0,2,0,8,0,0},
        {7,0,0,3,0,0,0,0,6},
    };

    int[,] givenExampleAlt = new int[9,9]{//0's are blank spaces
        {0,0,3,4,1,0,0,8,2},
        {7,0,0,0,6,3,1,5,0},
        {4,0,0,0,0,7,0,3,0},
        {0,0,6,3,7,8,0,0,0},
        {0,0,0,1,0,5,0,0,0},
        {0,0,0,6,4,2,3,0,0},
        {0,4,0,2,0,0,0,0,1},
        {0,6,7,9,8,0,0,0,3},
        {1,2,0,0,3,4,6,0,0},
    };
    void Start(){//This is called upon creation/program start
        InitCells();
        ResetBoard();
    }

    public void Set(int num, int row, int col){//sets space at position, to be accessed by solver
        cells[row,col].num = num;
        UpdateContraints((num != 0), row, col);
    }
    
    void UpdateContraints(bool addTo, int row, int col){//update the contraits of cells that are affected by this cell changing
        //check same row
        for (int i = 0; i < 9; i++)
            if (i != col)//updates only on ones that are empty, or might be empty again if it backtracks (so all non-given)
                cells[row, i].contraintsCount += (addTo) ? 1 : -1; //add or subtract from count

        //check same column
        for (int i = 0; i < 9; i++)
            if (i != row)//avoids updating the input cell
                cells[i, col].contraintsCount += (addTo) ? 1 : -1;

        //check the same box
        int boxRow = (row / 3) * 3;//find top left corner of this cells 3x3 box
        int boxCol = (col / 3) * 3;//use int truncation to round down
        for (int i = 0; i < 3; i++)
            for(int j = 0; j < 3; j++)
                if (!(row == boxRow + i || col == boxCol + j))//check a 3x3 from an offset, avoiding ones in already updated
                    cells[boxRow + i, boxCol + j].contraintsCount += (addTo) ? 1 : -1;
    }

    public bool Valid(int num, int row, int col){ //can you put num at row,col?
        //check same row
        for (int i = 0; i < 9; i++)
            if (cells[row, i].num == num)
                return false;//if theres a match then this input is not valid

        //check same column
        for (int i = 0; i < 9; i++)
            if (cells[i, col].num == num)
                return false;

        //check the same box
        int boxRow = (row / 3) * 3;//find top left corner of this cells 3x3 box
        int boxCol = (col / 3) * 3;//use int truncation to round down
        for (int i = 0; i < 3; i++)
            for(int j = 0; j < 3; j++)
                if (cells[boxRow + i, boxCol + j].num == num)//check a 3x3 from an offset
                    return false;

        return true;//reaches here is there where no matches found anywhere
    }

    public (int, int) NextEmpty() { // returns the row,col postion of the next
        (int, int) ret = (-1 , -1);//-1 represents a null value
        int highest = -10000;
        for (int row = 0; row < 9; row++)
            for(int col = 0; col < 9; col++)
                if(cells[row,col].num == 0 && cells[row,col].contraintsCount > highest){//search for cell with most constraints, for least backtracking
                    highest = cells[row,col].contraintsCount;
                    ret = (row, col);
                }
        return ret;
    }

    void InitCells(){//assigns UI text elements to the correct cells on this board, inits to 0 (blank)
        for (int i = 0; i < boxes.Length; i++){
            for (int j = 0; j < boxes[i].cells.Length; j++){
                int row = (i / 3) * 3 + j / 3; //remember int trunkation
                int col = (i % 3) * 3 + j % 3; //example: middle cell is cell 4(j) in box 4(i) and is at cell[4,4], the cell bellow is cell 7(j) of box 4(i) and is cell[5,4]
                cells[row,col].text = boxes[i].cells[j];
                cells[row,col].num = 0;
                cells[row,col].given = false;
                cells[row,col].contraintsCount = 0;
            }
        }
    }

    public void ResetBoard()
    {
        InitCells();
        SetAll(givenExampleAlt);
    }

    void SetAll(int[,] givens){//requires 9x9 array
        for(int i = 0; i < 9; i++){
            for(int j = 0; j < 9; j++){
                if (givens[i,j] != 0){//only set givens because of constraint updates
                    cells[i,j].given = true;
                    Set(givens[i,j],i,j);// 1 to 1 assign, givens will be set up for this
                }
            }
        }
    }

    public void Quit(){
        Application.Quit();
    }
}

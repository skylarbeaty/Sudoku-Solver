using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct cell{ //one of the 81 spaces of the board, handles text updates and cell data
    public int num { // requires 1-9 values, 0 for empty
        set{//also update text
            _num = value;
            if (num == 0)//leave blank for an empty
                text.text = "";
            else //set text to new value
                text.text = _num.ToString();
        }
        get{
            return _num;
        }
    }
    private int _num;
    //public bool given; // was this a given/clue value in the puzzle *not currently used
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

    void Start(){//This is called upon creation/program start
        InitCells();
        SetAll(givenExample);
    }

    public void Set(int num, int row, int col){//sets space at position, to be accessed by solver
        cells[row,col].num = num;
    }

    void InitCells(){//assigns UI text elements to the correct cells on this board, inits to 0 (blank)
        for (int i = 0; i < boxes.Length; i++){
            for (int j = 0; j < boxes[i].cells.Length; j++){
                int row = (i / 3) * 3 + j / 3; //remember int trunkation
                int col = (i % 3) * 3 + j % 3; //example: middle cell is cell 4(j) in box 4(i) and is at cell[4,4], the cell bellow is cell 7(j) of box 4(i) and is cell[5,4]
                cells[row,col].text = boxes[i].cells[j];
                cells[row,col].num = 0;
            }
        }
    }

    void SetAll(int[,] givens){//requires 9x9 array
        for(int i = 0; i < 9; i++){
            for(int j = 0; j < 9; j++){
                Set(givens[i,j],i,j);// 1 to 1 assign, givens will be set up for this
            }
        }
    }
}

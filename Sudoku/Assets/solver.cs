using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class solver : MonoBehaviour
{
    public board myBoard;
    void Start()
    {
        myBoard = GetComponent<board>();//requires this script be on the same object (in Unity) as the board script
    }

    public void StartSolve(){ // Unity UI button access. Button system wants a void function.
        if (Solve())
            print("solved");
        else
            print("not solved");
    }
    bool Solve(){
        (int row, int col) pos = myBoard.NextEmpty();
        if (pos.row == -1) //null check, board is full
            return true;//base case, will recusively return through ending function
        
        for (int i = 1; i <= 9; i++){//loop through all possible inputs
            if (myBoard.Valid(i, pos.row, pos.col)){
                myBoard.Set(i, pos.row, pos.col);//set as first balid number (will check others if control falls back here)

                if (Solve()) //recursive call
                    return true; //pass true through when board is solved
                
                myBoard.Set(0, pos.row, pos.col);//reset to empty, in case we fall to an earlier cell
            }
        }

        return false;//if it reaches here, no number is valid in this cell, need to change an earlier one

        // print("solve");
        // pos = myBoard.nextEmpty()
        // if pos is null
        //     return true
        
        // int row = pos.first
        // int col = pos.second

        // for i from 1 to 9
        //     if myBoard.Valid(i,row,col){
        //         myBoard.Set(i,row,col)

        //         if Solve(myBoard)
        //             return true;
                
        //         myBoard.Set(0,row,col)
        //     }
        // return false;
    }
}

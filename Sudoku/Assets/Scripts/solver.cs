using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class solver : MonoBehaviour
{
    public board myBoard;//using a ref to board, rather than passing as param, because Solve is called from a UI element
    void Start()
    {
        myBoard = GetComponent<board>();//requires this script be on the same object (in Unity) as the board script
    }

    public void StartSolve(){ // Unity UI button access. Button system wants a void function.
        if (Solve())
            print("solved");
        else
            print("ERROR: could not solve");
    }
    bool Solve(){
        (int row, int col) pos = myBoard.NextEmpty();
        if (pos.row == -1) //null check, board is full
            return true;//base case, will recusively return through, ending function
        
        for (int i = 1; i <= 9; i++){//loop through all possible inputs
            if (myBoard.Valid(i, pos.row, pos.col)){
                myBoard.Set(i, pos.row, pos.col);//set as first balid number (will check others if control falls back here)

                if (Solve()) //recursive call
                    return true; //pass true through when board is solved
                
                myBoard.Set(0, pos.row, pos.col);//reset to empty, in case we fall to an earlier cell
            }
        }

        return false;//if it reaches here, no number is valid in this cell, need to change an earlier one
    }

    public void StartSlowSolve(){
        StartCoroutine(SlowSolve());
    }
    IEnumerator SlowSolve(){//mimic of above algo, but made without recursion so that it could be slowed down for viewing
        float waitTime = 0.2f;
        (int row, int col) pos = myBoard.NextEmpty();
        Stack<(int num, int row, int col)> callStack = new Stack<(int num, int row, int col)>();//emulates recursive backtracking
        int iStart = 1; //first number to check for the cell. needed to mimic recursion
        while(pos.row != -1){
            for (int i = iStart; i <= 10; i++){
                if (i == 10){//couldnt find valid number for this 
                    myBoard.Set(0, pos.row, pos.col);//reset this cell
                    if (callStack.Count == 0){
                        print("ERROR: could not solve");
                        yield break;//exit coroutine (this function)
                    }
                    (int num, int row, int col) prev = callStack.Pop(); // get the previous cell
                    iStart = prev.num + 1; // start where it left off on this cell
                    pos = (prev.row, prev.col); // set current pos to prev pos
                    yield return new WaitForSeconds(waitTime);
                    break;//back to while loop with previous position
                }
                if (myBoard.Valid(i, pos.row, pos.col)){
                    myBoard.Set(i, pos.row, pos.col);
                    callStack.Push((i, pos.row, pos.col));//push this "frame" onto the "call stack" 
                    iStart = 1; //start checking numbers at 1 for new empty
                    pos = myBoard.NextEmpty();
                    yield return new WaitForSeconds(waitTime);
                    break;//back to while loop with next position
                }
            }
        }
        print("solved");
        yield return null;
    }
}

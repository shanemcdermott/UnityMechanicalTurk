using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystemGeneration : MonoBehaviour
{
    public int seed = 0;
    private List<Vector3> roads = new List<Vector3>();
    private string axiom;
    private float angle;
    private string currString;
    private float length = 2f;
    private int iterations;
    private Stack<TransformInfo> transformStack = new Stack<TransformInfo>();
    private Dictionary<char, string> rules = new Dictionary<char, string>();

    public enum lSystems
    {
        DragonCurve,
        FractalPlant,
        SierpinskiTri
    };
    public lSystems dropdown = lSystems.DragonCurve;
    // Use this for initialization
    void Start()
    {
        Random.InitState(seed);
        //*****RULES************
        if (dropdown == lSystems.DragonCurve)
        {
            //Dragon Curve
            rules.Add('X', "X+!YF+");
            rules.Add('Y', "-FX!-Y");
            angle = 90f;
            iterations = 12;
            axiom = "FX";
        }
        //
        else if (dropdown == lSystems.FractalPlant)
        {
            //Fractal Plant
            rules.Add('X', "F[-X][X]F[-X]+FX");
            rules.Add('F', "FF");
            axiom = "X";
            iterations = 6;
            angle = 25f;
        }
        //
        else if (dropdown == lSystems.SierpinskiTri)
        {
            //SierpinkskiTri
            rules.Add('F', "F-G+F+G-F");
            rules.Add('G', "GG");
            angle = 120f;
            iterations = 6;
            axiom = "F-G-G";
        }
        //
        roads.Add(transform.position);
        currString = axiom;
        angle = 90f;
        for (int i = 0; i < iterations; i++)
        {
            Generate();
        }
        DrawLines();
    }

    void DrawLines()
    {
        for (int i = 0; i < roads.Count - 1; i++)
        {
            Debug.DrawLine(roads[i], roads[i + 1], Color.black, 10000f, false);
        }
    }

    void Generate()
    {
        string newString = "";

        char[] stringCharacters = currString.ToCharArray();

        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];

            if (rules.ContainsKey(currentCharacter))
            {
                newString += rules[currentCharacter];
            }
            else
            {
                newString += currentCharacter.ToString();
            }
        }
        currString = newString;

        stringCharacters = currString.ToCharArray();
        int index = 0;
        for (int i = 0; i < stringCharacters.Length; i++)
        {
            char currentCharacter = stringCharacters[i];
            if (currentCharacter == 'F' || currentCharacter == 'G')
            {
                index++;
                transform.Translate(Vector3.forward * length);
                roads.Add(transform.position);
            }
            else if (currentCharacter == 'f')
            {
                transform.Translate(Vector3.forward * length);
            }
            else if (currentCharacter == '+')
            {
                transform.Rotate(Vector3.up * angle);
            }
            else if (currentCharacter == '-')
            {
                transform.Rotate(Vector3.up * -angle);
            }
            else if (currentCharacter == '[')
            {
                TransformInfo ti = new TransformInfo();
                ti.position = transform.position;
                ti.rotation = transform.rotation;
                transformStack.Push(ti);
            }
            else if (currentCharacter == ']')
            {
                TransformInfo ti = transformStack.Pop();
                transform.position = ti.position;
                transform.rotation = ti.rotation;
            }
            else if (currentCharacter == '!')//Insert Random Command
            {
                float num = Random.value;
                if(num < 0.1f)//Turn left
                {
                    stringCharacters.SetValue('-', i + 1);
                    i--;
                }
                else if( num < 0.2f)//Turn right
                {
                    stringCharacters.SetValue('+', i + 1);
                    i--;
                }
                else if(num < 0.3f)//Delete command
                {
                    stringCharacters.SetValue('n', i + 1);
                    i--;
                }
                // Anything else
                //Do nothing


                //last segment +=  new Vector3( Random.next(),0, Random.next() );
                //random needs to be 0,1,2,3 
            }
        }
    }

    private class TransformInfo
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}

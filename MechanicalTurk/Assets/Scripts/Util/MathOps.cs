using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathOps {


    public static Vector2 Midpoint(Vector2 A, Vector2 B)
    {
        return (A + B) * 0.5f;
    }

    public static float BisectorSlope(Vector2 A, Vector2 B)
    {
        Vector2 dif = A - B;
        if (dif.y == 0) return 0;
        return dif.x / dif.y;
    }

    public static float Slope(Vector2 A, Vector2 B)
    {
        Vector2 dif = B - A;
        if (dif.x == 0) return 0;
        return dif.y / dif.x;
    }

    //Const value for (m*x1 +y1) in y = m(x-x1) + y1
    public static float BisectorConst(Vector2 Midpoint, float slope)
    {
        return Midpoint.y - (slope * Midpoint.x);
    }

    public static Vector2 CircumCenter(Vector2 A, Vector2 B, Vector2 C)
    {
        Vector2 AB = MathOps.Midpoint(A, B);
        Vector2 AC = MathOps.Midpoint(A, C);
        Vector2 BC = MathOps.Midpoint(B, C);

        Vector2 MA = MathOps.Midpoint(AB, AC);
        Vector2 MC = MathOps.Midpoint(AC, BC);
        Vector2 centerPoint = MathOps.Midpoint(MA, MC);
        
        /*
        Vector2 MidpointAB = Midpoint(A, B);
        float BiSlopeAB = BisectorSlope(A, B);
        float BisectorABconst = BisectorConst(MidpointAB, BiSlopeAB);

        //Equation for Bisector is y = BiSlopeAB * x + BisectorABconst
        Vector2 MidpointAC = Midpoint(A, C);
        float BiSlopeAC = BisectorSlope(A, C);
        float BisectorACconst = BisectorConst(MidpointAC, BiSlopeAC);

        Vector2 centerPoint = new Vector2();
        centerPoint.x = (BisectorACconst - BisectorABconst) / (BiSlopeAB - BiSlopeAC);
        centerPoint.y = BiSlopeAB * centerPoint.x + BisectorABconst;
        */
        //y= BiSlopeAB * x + BisectorABconst
        //y= BislopeAC * x + BisectorACconst
        //BiSlopeAB * x + BisectorABconst = BislopeAC * x + BisectorACconst
        //BislopeAB * x = BiSlopeAC * x + BisectorACconst - BisectorABconst
        //BislopeAB * x - BiSlopeAC *x = BisectorACconst - BisectorABconst
        //x * (BislopeAB - BiSlopeAC) = ""
        //x = (BisectorACconst - BisectorABconst) / (BiSlopeAB - BiSlopeAC)

        return centerPoint;
    }

    public static float CircumRadius(Vector2 A, Vector2 B, Vector2 C)
    {
        float a = Vector2.Distance(A, B);
        float b = Vector2.Distance(A, C);
        float c = Vector2.Distance(B, C);
        float s = (a+b+c) / 2;
        float area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
        return a * b * c / (4 * area);
    }

    public static bool IsPointInCircumcircle(Vector2 P, Vector2 A, Vector2 B, Vector2 C)
    {
        Matrix4x4 mat = Matrix4x4.identity;
        mat[0, 0] = A.x - P.x;
        mat[0, 1] = A.y - P.y;
        mat[0, 2] = mat[0, 0] * mat[0, 0] + mat[0, 1] * mat[0, 1];
        mat[1, 0] = B.x - P.x;
        mat[1, 1] = B.y - P.y;
        mat[1, 2] = mat[1, 0] * mat[1, 0] + mat[1, 1] * mat[1, 1];
        mat[2, 0] = C.x - P.x;
        mat[2, 1] = C.y - P.y;
        mat[2, 2] = mat[2, 0] * mat[2, 0] + mat[2, 1] * mat[2, 1];

        return mat.determinant > 0;
        //Vector2 circumCenter = CircumCenter(A, B, C);
       // float circumRadius = CircumRadius(A, B, C);
       // return Vector2.Distance(circumCenter, p) <= circumRadius * circumRadius; 
    }

    
}

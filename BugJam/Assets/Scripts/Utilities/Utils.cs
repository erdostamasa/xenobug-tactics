using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Utils {
    public static string MatrixToString<T>(T[,] matrix) {
        string output = "";

        for (int x = 0; x < matrix.GetLength(0); x++) {
            for (int y = 0; y < matrix.GetLength(1); y++) {
                if (matrix[x, y] != null) {
                    output += matrix[x, y].ToString() + " ";
                }
                else {
                    output += "N/A";
                }
            }

            output += '\n';
        }

        return output;
    }

    public static string ListToString<T>(List<T> list) {
        string output = "";
        for (int i = 0; i < list.Count - 1; i++) {
            output += list[i].ToString() + " ";
        }

        output += list[list.Count - 1].ToString();
        return output;
    }

    public static string RemoveSpecialCharacters(string input) {
        StringBuilder sb = new StringBuilder();
        foreach (char c in input) {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public static T[,] RotateMatrix90Degrees<T>(T[,] arry) {
        int m = arry.GetLength(0);
        int n = arry.GetLength(1);
        if (m != n) {
            throw new ArithmeticException("Input matrix needs to be square matrix!");
        }

        int j = 0;
        int p = 0;
        int q = 0;
        int i = m - 1;
        T[,] rotatedArr = new T[m, n];

        //for (int i = m-1; i >= 0; i--)
        for (int k = 0; k < m; k++) {
            while (i >= 0) {
                rotatedArr[p, q] = arry[i, j];
                q++;
                i--;
            }

            j++;
            i = m - 1;
            q = 0;
            p++;
        }

        return rotatedArr;
    }

    // returns coordinates from the original matrix where the mask is ==1
    // needs the center of the mask matrix in original matrix coordinates
    public static List<(int, int)> MatrixMask<T>(T[,] originalMatrix, (int, int) maskCenterCoords, int[,] mask) {
        List<(int, int)> validCoordinates = new List<(int, int)>();

        int maskHalfSize = mask.GetLength(0) / 2;
        (int, int) start = (maskCenterCoords.Item1 - maskHalfSize, maskCenterCoords.Item2 - maskHalfSize);

        int maskx = 0;
        int masky = 0;
        for (int x = start.Item1; x < start.Item1 + mask.GetLength(0); x++) {
            for (int y = start.Item2; y < start.Item2 + mask.GetLength(1); y++) {
                //original matrix has a valid coordinate here
                //Debug.Log($"x:{x},y:{y},maskx:{maskx},masky:{masky} maskValue:{mask[maskx,masky]}");
                if (x >= 0 && x < originalMatrix.GetLength(0) && y >= 0 && y < originalMatrix.GetLength(1)) {
                    if (originalMatrix[x, y] != null) {
                        //mask marks this value
                        //Debug.Log($"VALID x:{x} y:{y}");
                        if (mask[maskx, masky] == 1) {
                            validCoordinates.Add((x, y));
                        }
                    }
                }

                masky++;
            }

            masky = 0;
            maskx++;
        }


        return validCoordinates;
    }
}
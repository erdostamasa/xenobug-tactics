using System;
using System.Collections.Generic;
using System.Text;

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
}
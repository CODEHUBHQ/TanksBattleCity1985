using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public static class BattleCityUtils
{
    public static int[] GetIntArray(int num)
    {
        List<int> listOfInts = new List<int>();

        while (num > 0)
        {
            listOfInts.Add(num % 10);

            num /= 10;
        }

        listOfInts.Reverse();

        return listOfInts.ToArray();
    }

    public static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();

        foreach (byte b in GetHash(inputString))
        {
            sb.Append(b.ToString("X2"));
        }

        return sb.ToString();
    }

    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
        {
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }

    public static string SplitCamelCase(string text)
    {
        string[] words = Regex.Matches(text, "(^[a-z]+|[A-Z]+(?![a-z])|[A-Z][a-z]+)")
            .OfType<Match>()
            .Select(m => m.Value)
            .ToArray();

        string result = string.Join(" ", words);

        return result;
    }

    public static Texture2D MergeTextures(List<Texture2D> textures)
    {
        Texture2D newTex = new Texture2D(416, 416);

        var index = 0;
        var col = 0;

        for (int i = 0; i < textures.Count; i++)
        {
            var tex = textures[i];

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    newTex.SetPixel((x + (i * 16)), -(y + (col * 16)), tex.GetPixel(x, y));
                }
            }

            index++;

            if (index == 26)
            {
                index = 0;
                col += 1;
            }
        }

        newTex.Apply();

        return newTex;
    }

    public static void SaveSpriteToPng(Sprite sprite, string filename)
    {
        if (string.IsNullOrEmpty(filename) || sprite == null) return;

        var bytes = sprite.texture.EncodeToPNG();
        var file = File.Open($"{Application.streamingAssetsPath}/MapsImgs/{filename}", FileMode.Create);
        var binary = new BinaryWriter(file);

        binary.Write(bytes);

        file.Close();
    }
}

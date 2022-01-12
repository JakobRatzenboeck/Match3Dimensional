using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SaveGame
{
    private List<TextAsset> textAssets;
    private TextAsset txt = new TextAsset();

    public SaveGame(string name, GameField field)
    {
        List<string> lines = new List<string>();
        List<Block> line = new List<Block>();
        textAssets = Resources.LoadAll<TextAsset>(GameManager.Constants._SavedFilesPath).ToList();

        List<string> names = textAssets.Select(ta => ta.name).ToList();
        List<int> indexes = new List<int>();
        char[] charint = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        for (int i = 0; i < names.Count; i++)
        {
            Debug.Log(names[i].Trim("0123456789".ToCharArray()));
            if (names[i].Trim("0123456789".ToCharArray()) == name)
            {
                indexes.Add(int.Parse(names[i].Split(charint)[1]));
            }
        }

        if (textAssets.Select(ta => ta.name.Trim("0123456789".ToCharArray()) == name).Count() > 0)
            name += textAssets.Select(ta => ta.name.Trim("0123456789".ToCharArray()) == name).Count();

        FileStream fileStream;
        if (GameManager.textname.Length == 0)
            fileStream = new FileStream(GameManager.Constants._FullSavedFilesPath + name + ".txt", FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
        else
            fileStream = new FileStream(GameManager.Constants._FullSavedFilesPath + name + ".txt", FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        StreamWriter sW = new StreamWriter(fileStream);

        sW.WriteLine(GetWriteGameData());

        for (int z = 0; z < GameManager.Constants.Z; z++)
            for (int y = 0; y < GameManager.Constants.Y; y++)
            {
                for (int x = 0; x < GameManager.Constants.X; x++)
                {
                    line.Add(field[x, y, z]);
                }
                sW.WriteLine(GetWriteLevelLine(line));
                line = new List<Block>();
            }

        sW.Close();
        fileStream.Close();
    }

    /// <summary>
    /// Converts the basic Stats/Data
    /// </summary>
    /// <returns>Returns the data to be writen down</returns>
    private string GetWriteGameData()
    {
        System.DateTime now = System.DateTime.Now;
        return $"{now.Day}.{now.Month}.{now.Year};{GameManager.seed};{GameManager.Constants.X},{GameManager.Constants.Y},{GameManager.Constants.Z};{GameManager.score};";
    }

    /// <summary>
    /// Writes down the blocks in rows (0-X,0,0 > 0-X,1,0 >> 0-X,Y,1)
    /// </summary>
    /// <param name="blockline">the line it readies up</param>
    /// <returns>The finished line to be writen into a file</returns>
    private string GetWriteLevelLine(List<Block> blockline)
    {
        string levelline = "";
        for (int i = 0; i < blockline.Count(); i++)
        {
            levelline += $"{ColorUtility.ToHtmlStringRGB(blockline[i].blockstats._Color)}_{blockline[i].blockstats._Type.ToString().Substring(0, 1)}/";
            levelline += $"{blockline[i]._position.x},{blockline[i]._position.y},{blockline[i]._position.z}";
            if (i < blockline.Count() - 1) levelline += "|";
        }
        return levelline;
    }
}

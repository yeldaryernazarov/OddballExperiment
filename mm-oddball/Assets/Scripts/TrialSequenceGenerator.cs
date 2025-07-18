using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TrialSequenceGenerator
{
    public static List<Trial> GenerateSequence(int nTrials, float oddballProb, int minStandard = 0, Vector2 isiRange = default)
    {
        if (isiRange == default) isiRange = new Vector2(1f, 2f);
        int oddballs = Mathf.FloorToInt(nTrials * oddballProb);
        int standards = nTrials - oddballs;

        if (oddballs > (nTrials - minStandard + 1) / 2)
        {
            Debug.LogError("Too many deviants to avoid repeats");
            return null;
        }

        List<string> stimTypes = new List<string>();
        for (int i = 0; i < minStandard; i++) stimTypes.Add("standard");

        List<string> remainder = new List<string>(new string[nTrials - minStandard]);
        for (int i = 0; i < remainder.Count; i++) remainder[i] = "standard";

        List<int> deviantPos = new List<int>();
        for (int i = 1; i < remainder.Count; i += 2) deviantPos.Add(i);
        deviantPos = Shuffle(deviantPos);
        for (int i = 0; i < oddballs; i++) remainder[deviantPos[i]] = "deviant";

        stimTypes.AddRange(remainder);

        List<Trial> trials = new List<Trial>();
        for (int i = 0; i < stimTypes.Count; i++)
        {
            trials.Add(new Trial
            {
                trialNum = i + 1,
                stimulusType = stimTypes[i],
                isi = Random.Range(isiRange.x, isiRange.y)
            });
        }
        return trials;
    }

    public static void SaveToCSV(List<Trial> trials, string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine("trial_num,stimulus_type,isi");
            foreach (var trial in trials)
                sw.WriteLine($"{trial.trialNum},{trial.stimulusType},{trial.isi}");
        }
    }

    private static List<int> Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
        return list;
    }
}

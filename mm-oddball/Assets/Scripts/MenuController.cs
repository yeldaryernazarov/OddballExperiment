using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class MenuController : MonoBehaviour
{
    public InputField trialCountField, oddballProbField;
    public Dropdown hapticDropdown;
    public ExperimentController experiment;
    private string path = "trialsequence.csv";

    public void OnGenerateClick()
    {
        int n = int.Parse(trialCountField.text);
        float p = float.Parse(oddballProbField.text);
        var trials = TrialSequenceGenerator.GenerateSequence(n, p, 20);
        TrialSequenceGenerator.SaveToCSV(trials, Application.persistentDataPath + "/" + path);
        Debug.Log("Sequence saved.");
    }

    public void OnStartClick()
    {
        var trials = LoadCSV();
        bool dual = hapticDropdown.value == 1;
        experiment.StartExperiment(trials, dual);
    }

    public List<Trial> LoadCSV()
    {
        string fullPath = Application.persistentDataPath + "/" + path;
        var lines = File.ReadAllLines(fullPath);
        var trials = new List<Trial>();
        for (int i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(',');
            trials.Add(new Trial
            {
                trialNum = int.Parse(cols[0]),
                stimulusType = cols[1],
                isi = float.Parse(cols[2])
            });
        }
        return trials;
    }
}

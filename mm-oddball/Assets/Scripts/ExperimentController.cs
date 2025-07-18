using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

public class ExperimentController : MonoBehaviour
{
    public GameObject fixationText, visualStandard, visualDeviant;
    public AudioSource audioStandard, audioDeviant;
    public string port1 = "COM3", port2 = "COM6";
    public bool dualHaptics = false;
    private SerialPort arduino1, arduino2;

    private List<Trial> trials;

    public void StartExperiment(List<Trial> loadedTrials, bool useDual)
    {
        dualHaptics = useDual;
        trials = loadedTrials;
        StartCoroutine(RunTrials());
    }

    IEnumerator RunTrials()
    {
        InitArduino();

        foreach (var trial in trials)
        {
            fixationText.SetActive(true);
            visualStandard.SetActive(false);
            visualDeviant.SetActive(false);
            yield return new WaitForSeconds(trial.isi);

            fixationText.SetActive(false);
            GameObject stim = trial.stimulusType == "standard" ? visualStandard : visualDeviant;
            stim.SetActive(true);

            if (trial.stimulusType == "standard") audioStandard.Play();
            else audioDeviant.Play();

            SendHaptic(trial.stimulusType);
            yield return new WaitForSeconds(0.2f);

            stim.SetActive(false);
            fixationText.SetActive(true);
        }

        CloseArduino();
        Debug.Log("Experiment done.");
    }

    void InitArduino()
    {
        arduino1 = new SerialPort(port1, 9600);
        arduino1.Open();
        if (dualHaptics)
        {
            arduino2 = new SerialPort(port2, 9600);
            arduino2.Open();
        }
    }

    void SendHaptic(string type)
    {
        char cmd = type == "standard" ? 'S' : 'D';
        arduino1?.Write(cmd.ToString());
        if (dualHaptics) arduino2?.Write(cmd.ToString());
    }

    void CloseArduino()
    {
        if (arduino1?.IsOpen == true) arduino1.Close();
        if (arduino2?.IsOpen == true) arduino2.Close();
    }
}

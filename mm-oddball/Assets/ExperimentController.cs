using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;

public class ExperimentController : MonoBehaviour {
    public GameObject Fixation, VisualStd, VisualDev;
    public AudioSource AudioStd, AudioDev;
    public string Port1="COM3", Port2="COM6";
    SerialPort A1, A2;
    List<Trial> trials;
    bool dualHaptic;

    public void Initialize(List<Trial> seq, bool twoHaptics) {
        trials = seq; dualHaptic = twoHaptics;
    }

    public void Begin() => StartCoroutine(Run());

    IEnumerator Run() {
        // 1) Open serial
        A1 = new SerialPort(Port1,9600); A1.Open();
        if (dualHaptic) { A2 = new SerialPort(Port2,9600); A2.Open(); }

        foreach (var t in trials) {
            // Fixation
            VisualStd.SetActive(false); VisualDev.SetActive(false);
            Fixation.SetActive(true);
            yield return new WaitForSeconds(t.isi);

            // Stimulus
            Fixation.SetActive(false);
            bool isStd = t.stimulusType=="standard";
            (isStd ? VisualStd : VisualDev).SetActive(true);
            (isStd ? AudioStd : AudioDev).Play();

            // Haptic pulse
            char cmd = isStd ? 'S' : 'D';
            A1.Write(cmd.ToString());
            if (dualHaptic) A2.Write(cmd.ToString());

            yield return new WaitForSeconds(0.2f);

            (isStd ? VisualStd : VisualDev).SetActive(false);
        }

        // 2) Close serial
        A1.Close();
        if (dualHaptic) A2.Close();

        Debug.Log("Experiment complete!");
    }
}

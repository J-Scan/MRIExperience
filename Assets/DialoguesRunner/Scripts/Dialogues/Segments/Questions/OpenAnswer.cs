using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class OpenAnswer : MonoBehaviour, IQuestion
{
    [SerializeField] private GameObject onBg;
    [SerializeField] private GameObject offBg;
    [SerializeField] private GameObject playbackButton;
    [SerializeField] private GameObject playbackButtonOnBg;
    [SerializeField] private GameObject playbackButtonoOffBg;
    [SerializeField] private AudioSource audioSource;

    private AudioClip clipRecord;

    private bool isRecording = false;
    private const int maxRecordingDuration = 600;
    private float answerTime = -1f;

    private string microphone;


    public string GetQuestionType()
    {
        return "OpenAnswer";
    }

    public void DisplayQuestion(GameObject questionGO)
    {
        questionGO.SetActive(true);
    }

    public void OnButtonClicked()
    {
        if (audioSource == null) return;

        onBg.SetActive(!onBg.activeSelf);
        offBg.SetActive(!offBg.activeSelf);

        microphone = GetMicrophoneDevice();
        if (microphone == null)
        {
            Debug.LogWarning("No microphone found. Please connect a microphone.");
            return;
        }

        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    public void OnPlaybackButtonClicked()
    {
        if (audioSource == null) return;

        playbackButtonOnBg.SetActive(!playbackButtonOnBg.activeSelf);
        playbackButtonoOffBg.SetActive(!playbackButtonoOffBg.activeSelf);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (playbackButtonOnBg.activeSelf)
        {
            audioSource.PlayOneShot(clipRecord);
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void StartRecording()
    {
        if (!isRecording && microphone != null)
        {
            audioSource.Stop();
            audioSource.clip = Microphone.Start(microphone, false, maxRecordingDuration, 44100);
            isRecording = true;
            Debug.Log($"Recording started on device: {microphone}");
        }
        else
        {
            Debug.LogWarning("Recording is already in progress or no microphone available.");
        }
    }

    private void SetAnswerTime()
    {
        answerTime = Time.time; // Capture the current time since the start of the game
        Debug.Log($"Answer time set to: {answerTime}");
    }

    private void StopRecording()
    {
        if (isRecording)
        {
            int currentPosition = Microphone.GetPosition(microphone);
            if (currentPosition <= 0)
            {
                Debug.LogWarning("Recording was empty, no audio data captured.");
                Microphone.End(microphone);
                return;
            }

            Microphone.End(microphone);
            isRecording = false;
            Debug.Log("Recording stopped successfully.");

            // Trim the audio clip to the actual length
            SetAnswerTime();
            clipRecord = audioSource.clip;
            TrimAudioClip(currentPosition);
            playbackButton.SetActive(true);
        }
    }

    private string GetMicrophoneDevice()
    {
        if (Microphone.devices.Length > 0)
        {
            foreach (var device in Microphone.devices)
            {
                if (device.ToLower().Contains("oculus"))
                {
                    return device;
                }
            }
            return Microphone.devices[0];
        }
        return null;
    }

    private void TrimAudioClip(int currentPosition)
    {
        if (clipRecord != null)
        {
            Debug.Log($"Recorded samples: {clipRecord.samples}. Channels: {clipRecord.channels}. Frequency: {clipRecord.frequency}.");
            int recordingPosition = Mathf.Min(currentPosition, clipRecord.samples);
            float[] samples = new float[clipRecord.samples];
            clipRecord.GetData(samples, 0);

            float[] trimmedSamples = new float[recordingPosition];
            Array.Copy(samples, trimmedSamples, recordingPosition);

            clipRecord = AudioClip.Create("TrimmedClip", recordingPosition, clipRecord.channels, clipRecord.frequency, false);
            clipRecord.SetData(trimmedSamples, 0);

            Debug.Log($"Trimmed clip created with samples: {recordingPosition}");
        }
    }

    public string GetAnswer()
    {
        if (clipRecord == null || clipRecord.samples == 0)
        {
            Debug.LogWarning("No audio data to save.");
            return "No Audio";
        }
;
        return clipRecord.name;
    }

    public void SaveAudioAnswer()
    {

        DateTime now = DateTime.Now;
        string dirName = "ParticipantsData";
        string formattedDateTime = now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filePath = Path.Combine(dirName, $"{formattedDateTime}_audio.wav");

        Directory.CreateDirectory(dirName);


        SaveWav(clipRecord, filePath);
    }

    public float GetAnswerTime()
    {
        return isRecording ? -1f : Time.time - answerTime;
    }

    private static void SaveWav(AudioClip clip, string filePath)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip is null, cannot save.");
            return;
        }

        byte[] wavData = ConvertAudioClipToWav(clip);
        File.WriteAllBytes(filePath, wavData);
    }

    private static byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        short[] pcmSamples = new short[samples.Length];

        for (int i = 0; i < samples.Length; i++)
        {
            pcmSamples[i] = (short)(samples[i] * short.MaxValue);
        }

        byte[] wavHeader = CreateWavHeader(pcmSamples.Length, clip.channels, clip.frequency);
        byte[] wavData = new byte[wavHeader.Length + pcmSamples.Length * sizeof(short)];
        Buffer.BlockCopy(wavHeader, 0, wavData, 0, wavHeader.Length);
        Buffer.BlockCopy(pcmSamples, 0, wavData, wavHeader.Length, pcmSamples.Length * sizeof(short));

        return wavData;
    }

    private static byte[] CreateWavHeader(int numSamples, int numChannels, int sampleRate)
    {
        int byteRate = sampleRate * numChannels * sizeof(short);
        int blockAlign = numChannels * sizeof(short);
        int dataSize = numSamples * numChannels * sizeof(short);

        using (MemoryStream stream = new MemoryStream(44))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(new[] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + dataSize);
                writer.Write(new[] { 'W', 'A', 'V', 'E' });
                writer.Write(new[] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)numChannels);
                writer.Write(sampleRate);
                writer.Write(byteRate);
                writer.Write((short)blockAlign);
                writer.Write((short)16);
                writer.Write(new[] { 'd', 'a', 't', 'a' });
                writer.Write(dataSize);
            }
            return stream.ToArray();
        }
    }

    private void OnDisable()
    {
        StopRecording();
        SaveAudioAnswer();
    }
}

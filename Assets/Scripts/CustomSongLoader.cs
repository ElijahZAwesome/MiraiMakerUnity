﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using NAudio;
using NAudio.Wave;
using UnityEngine.Networking;

[Serializable]
public class LevelClass
{
    public string levelName;
    public string songPath;
    public float bpm;
    public float firstBeatOffset;
    public int beatsInAdvance;
    public int pathBeatsInAdvance;
    public float fadeOffsetInBeats;
    public Vector2[] notes;
    public Vector2[] cameraKeyframes;
    public Vector3[] path;

    public static LevelClass CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LevelClass>(jsonString);
    }
}

public class CustomSongLoader : MonoBehaviour
{

    public MusicHandler musicHandler;
    public string filePath;

    void Awake()
    {
        //WriteLevelToJSON();
        LoadLevelFromJSON();
        print(Path.GetDirectoryName(Application.dataPath));
    }

    public void LoadLevelFromJSON()
    {
        StreamReader reader = new StreamReader(filePath);
        string json = reader.ReadToEnd();
        reader.Close();
        LevelClass level = JsonUtility.FromJson<LevelClass>(json);
        musicHandler.bpm = level.bpm;
        musicHandler.levelName = level.levelName;
        musicHandler.notes = level.notes;
        musicHandler.cameraKeyframes = level.cameraKeyframes;
        musicHandler.fadeOffsetInBeats = level.fadeOffsetInBeats;
        musicHandler.firstBeatOffset = level.firstBeatOffset;
        musicHandler.beatsInAdvance = level.beatsInAdvance;
        musicHandler.pathBeatsInAdvance = level.pathBeatsInAdvance;
        musicHandler.gamePath.GetComponent<MotionPath>().controlPoints = level.path;
        musicHandler.songPath = level.songPath;
        string url = "";
        url += level.songPath;
        StartCoroutine(LoadAudio(url, Path.GetFileNameWithoutExtension(level.songPath)));
    }

    IEnumerator LoadAudio(string url, string name)
    {
        string furl = "file:///" + url;
        AudioType type;
        print(Path.GetExtension(url).ToLower());
        switch(Path.GetExtension(url).ToLower()) {
            case ".mp3":
                type = AudioType.MPEG;
                break;
            case ".wav":
                type = AudioType.WAV;
                break;
            case ".ogg":
                type = AudioType.OGGVORBIS;
                break;
            case ".xma":
                type = AudioType.XMA;
                break;
            default:
                type = AudioType.UNKNOWN;
                break;
        }
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(furl, type))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip ac;
                switch (type)
                {
                    case AudioType.MPEG:
                        ac = NAudioPlayer.FromMp3Data(www.downloadHandler.data);
                        break;
                    default:
                        ac = DownloadHandlerAudioClip.GetContent(www);
                        break;
                }
                ac.name = name;
                musicHandler.song = ac;
                musicHandler.BeginGame();
            }
        }
    }


    public void WriteLevelToJSON()
    {
        StreamWriter writer = new StreamWriter(filePath);
        writer.Write(LevelToJson());
        writer.Close();
    }

    string LevelToJson()
    {
        LevelClass level = new LevelClass();
        level.levelName = musicHandler.levelName;
        level.bpm = musicHandler.bpm;
        level.notes = musicHandler.notes;
        level.cameraKeyframes = musicHandler.cameraKeyframes;
        level.firstBeatOffset = musicHandler.firstBeatOffset;
        level.fadeOffsetInBeats = musicHandler.fadeOffsetInBeats;
        level.beatsInAdvance = musicHandler.beatsInAdvance;
        level.pathBeatsInAdvance = musicHandler.pathBeatsInAdvance;
        level.songPath = musicHandler.songPath;
        level.path = musicHandler.gamePath.GetComponent<MotionPath>().controlPoints;
        string json = JsonUtility.ToJson(level);
        return json;
    }
}
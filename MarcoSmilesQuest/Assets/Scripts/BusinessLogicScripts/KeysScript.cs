using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeysScript : MonoBehaviour
{

    // All keys
    private List<GameObject> _keys;

    // Only the selected keys

    private static List<GameObject> _selectedKeys = new List<GameObject>();
    public static List<GameObject> SelectedKeys
    {
        get { 
            // Sort the list of selected keys by Pitch and Octave
            _selectedKeys = _selectedKeys.OrderBy(k => new Note(k.name).Octave).ThenBy(k => new Note(k.name).Pitch).ToList();
            return _selectedKeys;
        }
        private set { _selectedKeys = value; }
    }

    private GameObject _firstSelectedKey;
    public GameObject FirstSelectedKey
    {
        get => _firstSelectedKey;
        set
        {
            _firstSelectedKey = value;
            if (_firstSelectedKey != null)
                _firstSelectedKey.GetComponent<Image>().color = GetSelectedColor(_firstSelectedKey.name);
        }
    }

    private GameObject _lastSelectedKey;
    public GameObject LastSelectedKey
    {
        get => _lastSelectedKey;
        set
        {
            _lastSelectedKey = value;
            if (_lastSelectedKey != null)
                _lastSelectedKey.GetComponent<Image>().color = GetSelectedColor(_lastSelectedKey.name);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> allChildren = GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToList();
        _keys = allChildren.Where(k => k.tag == "Key").ToList();
        _keys = _keys.OrderByDescending(k => new Note(k.name).Octave).ThenBy(k => new Note(k.name).Pitch).ToList();

        foreach (GameObject key in _keys)
        {
            if (!key.name.Contains("#")) // Ignore diesis, for now
            {
                Toggle keyToggle = key.GetComponent<Toggle>();
                keyToggle.onValueChanged.AddListener(delegate
                {
                    SelectKey(keyToggle);
                });
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnEnable()
    {
        ResetKeys();
        // This sucks, I'm really sorry :(
        if (NotesList.Notes != null && NotesList.Notes.Length > 0)
        {
            foreach (Note note in NotesList.Notes)
            {
                if (note.Equals(Note.GetPause()))
                {
                    continue;
                }
                foreach (GameObject key in _keys)
                {
                    if (key.name == note.ToString())
                    {
                        key.GetComponent<Image>().color = GetSelectedColor(key.name);
                        SelectedKeys.Add(key);
                        // Se è il primo elemento di NotesList.Notes, lo seleziono come primo tasto
                        if (note == NotesList.Notes[0])
                        {
                            FirstSelectedKey = key;
                        }
                        // Se è il penultimo (l'ultimo è la pausa) elemento di NotesList.Notes, lo seleziono come ultimo tasto
                        if (note == NotesList.Notes[NotesList.Notes.Length - 1 - 1])
                        {
                            LastSelectedKey = key;
                        }
                    }
                }
            }
        }
    }

    public void SelectKey(Toggle keyToggle)
    {
        if (keyToggle.isOn)
        {
            if (FirstSelectedKey == null && LastSelectedKey == null)
            {
                FirstSelectedKey = keyToggle.gameObject;
            }
            else if (FirstSelectedKey != null && LastSelectedKey == null)
            {
                LastSelectedKey = keyToggle.gameObject;
                ComputeSelectedKeys();
            }
            else
            {
                ResetKeys();
                FirstSelectedKey = keyToggle.gameObject;
                LastSelectedKey = null;
            }
        }
    }

    private void ComputeSelectedKeys()
    {
        if (FirstSelectedKey != null && LastSelectedKey != null)
        {
            float firstSelectedKeyPositionX = FirstSelectedKey.GetComponent<Transform>().position.x;
            float lastSelectedKeyPositionX = LastSelectedKey.GetComponent<Transform>().position.x;
            foreach (GameObject key in _keys)
            {
                float keyPositionX = key.GetComponent<Transform>().position.x;
                if ((keyPositionX >= firstSelectedKeyPositionX && keyPositionX <= lastSelectedKeyPositionX) ||
                    (keyPositionX <= firstSelectedKeyPositionX && keyPositionX >= lastSelectedKeyPositionX))
                {
                    if (!key.name.Contains("#")) // Ignore diesis, for now
                    {
                        key.GetComponent<Image>().color = GetSelectedColor(key.name);
                        SelectedKeys.Add(key);
                    }
                }
            }
        }
    }

    public void ResetKeys()
    {
        foreach (GameObject key in _keys)
        {
            key.GetComponent<Image>().color = GetResetColor(key.name);
        }
        SelectedKeys.Clear();
        FirstSelectedKey = null;
        LastSelectedKey = null;
    }

    private static Color GetResetColor(string keyName)
    {
        if (keyName.Contains("#"))
        {
            return Color.black;
        }
        else
        {
            return Color.white;
        }
    }

    private static Color GetSelectedColor(string keyName)
    {
        Color color;
        if (keyName.Contains("#"))
        {
            ColorUtility.TryParseHtmlString("#2D5B2A", out color);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#3F903A", out color);
        }
        return color;
    }

}

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class NotesList
{
    public static Note[] Notes;

    public static void CreateEmptyNotesList(int length)
    {
        Notes = new Note[length + 1];
        Notes[length] = Note.GetPause();
    }

    public static void CreateNotesList(Note startNote, int length)
    {
        Notes = new Note[length + 1];
        Notes[0] = startNote;
        for (int i = 1; i < length; i++)
        {
            Notes[i] = Notes[i - 1].ComputeNextNote();
        }
        Notes[length] = Note.GetPause();
    }

    public static Note GetFirstNote()
    {
        return Notes[0];
    }

    public static Note GetRandomNote()
    {
        System.Random random = new System.Random();

        // Total weight of all notes
        double? totalWeight = Notes?.Sum(note => 1.0 / (note.GuessedCounter + 1));

        // The weight of the note to be selected
        double? randomValue = random.NextDouble() * totalWeight;

        // Select the note
        double cumulativeWeight = 0.0;
        if (Notes != null)
        {
            foreach (Note note in Notes)
            {
                cumulativeWeight += 1.0 / (note.GuessedCounter + 1);
                if (randomValue <= cumulativeWeight)
                {
                    return note;
                }
            }
        }

        // JIC
        return new Note(Note.PitchEnum.DO, Note.OctaveEnum.FOUR);
    }

    public static string ToJson()
    {
        return JsonConvert.SerializeObject(new List<Note>(Notes));
    }

    public static Note[] FromJson(string json)
    {
        return JsonConvert.DeserializeObject<List<Note>>(json).ToArray();
    }

}
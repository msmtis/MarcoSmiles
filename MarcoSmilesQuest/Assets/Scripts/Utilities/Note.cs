using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Note
{
    [Serializable]
    public enum PitchEnum
    {
        DO,
        // DO_SHARP,
        RE,
        // RE_SHARP,
        MI,
        FA,
        // FA_SHARP,
        SOL,
        // SOL_SHARP,
        LA,
        // LA_SHARP,
        SI,
        PAUSE
    }

    [Serializable]
    public enum OctaveEnum
    {
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN
    }

    [SerializeField]
    public PitchEnum Pitch;
    [SerializeField]
    public OctaveEnum Octave;
    public int GuessedCounter;

    public Note()
    {
    }

    public Note(string pitch, string octave)
    {
        Pitch = StringToPitch(pitch);
        Octave = StringToOctave(octave);
        GuessedCounter = 0;
    }

    public Note(string note)
    {
        Pitch = StringToPitch(note.Substring(0, note.Length - 1));
        Octave = StringToOctave(note.Substring(note.Length - 1));
        GuessedCounter = 0;
    }

    public Note(PitchEnum pitch, OctaveEnum octave)
    {
        Pitch = pitch;
        Octave = octave;
        GuessedCounter = 0;
    }

    public static Note GetPause()
    {
        return new Note(PitchEnum.PAUSE, OctaveEnum.ZERO);
    }

    private static PitchEnum GetRandomPitch()
    {
        Array values = Enum.GetValues(typeof(PitchEnum));
        System.Random random = new System.Random();
        return (PitchEnum)values.GetValue(random.Next(values.Length));
    }

    private static OctaveEnum GetRandomOctave()
    {
        Array values = Enum.GetValues(typeof(OctaveEnum));
        System.Random random = new System.Random();
        return (OctaveEnum)values.GetValue(random.Next(values.Length));
    }

    private static string PitchToString(PitchEnum note)
    {
        switch (note)
        {
            case PitchEnum.DO:
                return "DO";
            // case PitchEnum.DO_SHARP:
            //    return "DO#";
            case PitchEnum.RE:
                return "RE";
            // case PitchEnum.RE_SHARP:
                // return "RE#";
            case PitchEnum.MI:
                return "MI";
            case PitchEnum.FA:
                return "FA";
            // case PitchEnum.FA_SHARP:
                // return "FA#";
            case PitchEnum.SOL:
                return "SOL";
            // case PitchEnum.SOL_SHARP:
                // return "SOL#";
            case PitchEnum.LA:
                return "LA";
            // case PitchEnum.LA_SHARP:
                // return "LA#";
            case PitchEnum.SI:
                return "SI";
            case PitchEnum.PAUSE:
                return "PAUSE";
            default:
                return "Error somewhere :( Check the logs!";
        }
    }

    private static string PitchEuropeanToInternational(string pitch)
    {
        switch (pitch)
        {
            case "DO":
                return "C";
            case "DO#":
                return "C#";
            case "RE":
                return "D";
            case "RE#":
                return "D#";
            case "MI":
                return "E";
            case "FA":
                return "F";
            case "FA#":
                return "F#";
            case "SOL":
                return "G";
            case "SOL#":
                return "G#";
            case "LA":
                return "A";
            case "LA#":
                return "A#";
            case "SI":
                return "B";
            case "PAUSE":
                return "PAUSE";
            default:
                return "Error somewhere :( Check the logs!";
        }
    }

    private static PitchEnum StringToPitch(string pitch)
    {
        switch (pitch)
        {
            case "DO":
                return PitchEnum.DO;
            case "DO#":
                // return PitchEnum.DO_SHARP;
            case "RE":
                return PitchEnum.RE;
            case "RE#":
                // return PitchEnum.RE_SHARP;
            case "MI":
                return PitchEnum.MI;
            case "FA":
                return PitchEnum.FA;
            case "FA#":
                // return PitchEnum.FA_SHARP;
            case "SOL":
                return PitchEnum.SOL;
            case "SOL#":
                // return PitchEnum.SOL_SHARP;
            case "LA":
                return PitchEnum.LA;
            case "LA#":
                // return PitchEnum.LA_SHARP;
            case "SI":
                return PitchEnum.SI;
            case "PAUSE":
                return PitchEnum.PAUSE;
            default:
                throw new Exception("Invalid pitch!");
        }
    }

    private static OctaveEnum StringToOctave(string octave)
    {
        switch (octave)
        {
            case "0":
                return OctaveEnum.ZERO;
            case "PAUSE":
                return OctaveEnum.ZERO;
            case "1":
                return OctaveEnum.ONE;
            case "2":
                return OctaveEnum.TWO;
            case "3":
                return OctaveEnum.THREE;
            case "4":
                return OctaveEnum.FOUR;
            case "5":
                return OctaveEnum.FIVE;
            case "6":
                return OctaveEnum.SIX;
            case "7":
                return OctaveEnum.SEVEN;
            default:
                throw new Exception("Invalid octave!");
        }
    }

    private static string OctaveToString(OctaveEnum octave)
    {
        return ((int)octave).ToString();
    }

    public Note ComputeNextNote()
    {
        if (Pitch == PitchEnum.SI && Octave == OctaveEnum.SEVEN)
        {
            throw new Exception("No more notes available!");
        }
        PitchEnum nextPitch = (PitchEnum)((int)(Pitch + 1) % (Enum.GetValues(typeof(PitchEnum)).Length - 1));
        OctaveEnum nextOctave = Pitch == PitchEnum.SI ? Octave + 1 : Octave;
        return new Note(nextPitch, nextOctave);
    }

    private static int ComputeDistance(Note startNote, Note endNote)
    {
        return ((int)(endNote.Pitch - startNote.Pitch)) + ((int)(endNote.Octave - startNote.Octave) * (Enum.GetValues(typeof(PitchEnum)).Length - 1));
    }

    public int ComputeDistance()
    {
        if (Pitch == PitchEnum.PAUSE && Octave == OctaveEnum.ZERO)
        {
            return NotesList.Notes.Length - 1;
        }
        Note startNote = NotesList.GetFirstNote();
        return ComputeDistance(startNote, this);
    }

    public override string ToString()
    {
        if (Pitch == PitchEnum.PAUSE && Octave == OctaveEnum.ZERO)
        {
            return "PAUSE";
        }
        return PitchToString(Pitch) + OctaveToString(Octave);
    }

    public string ToStringInternational()
    {
        if (Pitch == PitchEnum.PAUSE && Octave == OctaveEnum.ZERO)
        {
            return "PAUSE";
        }
        return PitchEuropeanToInternational(PitchToString(Pitch)) + OctaveToString(Octave);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        Note note = (Note)obj;
        return Pitch == note.Pitch && Octave == note.Octave;
    }

    public override int GetHashCode()
    {
        return 0;
    }

}

using System;
using System.Collections.Generic;

[Serializable]
public class RequestWrapper
{
    public List<HandWrapper> LeftHandWrappers;
    public List<HandWrapper> RightHandWrappers;
    public int Note;


    public RequestWrapper()
    {
        LeftHandWrappers = new List<HandWrapper>();
        RightHandWrappers = new List<HandWrapper>();
    }

    public RequestWrapper(int note)
    {
        LeftHandWrappers = new List<HandWrapper>();
        RightHandWrappers = new List<HandWrapper>();
        Note = note;
    }
}
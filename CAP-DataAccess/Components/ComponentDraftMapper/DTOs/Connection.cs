﻿namespace Components.ComponentDraftMapper.DTOs
{
    public class Connection
    {
        public int fromPinNr { get; set; }
        public int toPinNr { get; set; }
        public float magnitude { get; set; }
        public float wireLengthNM { get; set; }
    }
}
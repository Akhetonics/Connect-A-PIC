namespace CAP_Core.ExternalPorts
{
    public class UsedInput
    {
        public UsedInput(ExternalInput input, Guid attachedComponentPinId)
        {
            Input = input;
            AttachedComponentPinId = attachedComponentPinId;
        }
        public ExternalInput Input { get; }
        public Guid AttachedComponentPinId { get;}

    }
}
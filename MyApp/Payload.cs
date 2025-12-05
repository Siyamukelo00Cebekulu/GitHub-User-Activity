
public class Payload
{
    public long Repository_Id { get; set; }
    public long Push_Id { get; set; }
    public string Ref { get; set; }
    public string Head { get; set; }
    public string Before { get; set; }

    // PushEvent specific
    public List<Commit> Commits { get; set; }


    // CreateEvent fields (only appear sometimes)
    public string Ref_Type { get; set; }
    public string Full_Ref { get; set; }
    public string Master_Branch { get; set; }
    public string Description { get; set; }
    public string Pusher_Type { get; set; }
}
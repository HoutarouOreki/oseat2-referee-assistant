namespace oSEAT2RefereeAssistant
{
    public class BeatMap
    {
        public readonly string Code;
        public readonly string Name;
        public readonly string Id;

        /// <param name="mapTableRow">CODE(tab)NAME(tab)!mp map ID</param>
        public BeatMap(string mapTableRow)
        {
            var temp = mapTableRow.Split('\t');
            Code = temp[0];
            Name = temp[1];
            Id = temp[2];
        }

        public override string ToString()
        {
            return $"{Code} : {Name} : {Id}";
        }
    }
}

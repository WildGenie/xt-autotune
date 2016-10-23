namespace AutoTune.Settings {

    public class Entry<T> {

        public T Item { get; set; }
        public string Id { get; set; }

        public Entry() { }
        public Entry(string id, T item) {
            Id = id;
            Item = item;
        }
    }
}

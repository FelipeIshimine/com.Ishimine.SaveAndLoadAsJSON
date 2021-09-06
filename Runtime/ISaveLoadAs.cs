using Leguar.TotalJSON;

namespace SaveSystem
{
    public interface ISaveLoadAs<T>
    {
        //Retorna los datos que queremos almacenar
        T GetSaveData();
        //Carga los datos en data
        void LoadSaveData(T data);
    }
    
}
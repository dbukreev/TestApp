using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System.Collections.Generic    //а почему нет?)
{
   
    class ExtendedDictionary<TId, TName, TValue>: IEnumerable<KeyValuePair<KeysPair<TId, TName>, TValue>>
    {
        
        /*Идея простая: всего три словаря. Первый содержит парный ключ (структура из Id и name) и значение. Он обеспечивает наибыстрейший селективный доступ.
         Второй содержит Id как ключ и List<TValue> как значение - это список элементов с таким Id, невзирая на Name. Добавление в основной словарь и кэши 
         * будет происходить при добавлении элемента через [].
         Третий - тоже, но для Name.
         Таким образом, селективный доступ и выборки по каждому из ключей имеют вычислительную сложность O(1)*/

        Dictionary<KeysPair<TId, TName>, TValue> full_dic;
        Dictionary<TId, List<TValue>> fast_acces_by_id;
        Dictionary<TName, List<TValue>> fast_acces_by_name;

        public int Count
        {
            get{ return full_dic.Count(); }
        }

        public ExtendedDictionary(int size=64)
        {
            size = size > 0 ? size : 64;
            full_dic = new Dictionary<KeysPair<TId, TName>,TValue>(size);
            fast_acces_by_id = new Dictionary<TId,List<TValue>>(size);
            fast_acces_by_name = new Dictionary<TName, List<TValue>>(size);
            
        }

        //Реализуем обращение через квадратные скобки
        public TValue this[TId id, TName name]
        {
            set
            {
                full_dic[new KeysPair<TId, TName>(id, name)] = value;   //Добавляем запись в основной словарь

                if(fast_acces_by_id.ContainsKey(id)) fast_acces_by_id[id].Add(value);   //Быстрый кэш для id
                    else fast_acces_by_id[id] = new List<TValue>() { value };

                if (fast_acces_by_name.ContainsKey(name)) fast_acces_by_name[name].Add(value);  //Быстрый кэш для Name
                    else fast_acces_by_name[name] = new List<TValue>() { value };
            }
            get
            {
                return full_dic[new KeysPair<TId, TName>(id, name)];    //Обычный селективный доступ, подойдет основной слвоарь
            }
        }

        public TValue[] GetByName(TName name)
        {
            return fast_acces_by_name[name].ToArray();      //Ну как, быстро? ))
        }

        public TValue[] GetById(TId id)
        {
            return fast_acces_by_id[id].ToArray();          //Вообще еще быстрее возвращать сразу List, но тогда я нарушу инкапсуляцию.
        }

        public bool TryRemove(TId id, TName name)
        {
            if (full_dic.ContainsKey(new KeysPair<TId, TName>(id, name)))   //Если такой ключ есть
            {
                var tmp_key = new KeysPair<TId, TName>(id, name);

                if (fast_acces_by_id[id].Count > 1)             //Убираем его из быстрого доступа по id
                    fast_acces_by_id[id].Remove(full_dic[tmp_key]);
                else fast_acces_by_id.Remove(id);

                if (fast_acces_by_name[name].Count > 1)         //Убираем его из быстрого доступа по name
                    fast_acces_by_name[name].Remove(full_dic[tmp_key]);
                else fast_acces_by_name.Remove(name);

                full_dic.Remove(tmp_key);                       //Убираем его окончательно из основного словаря.

                return true;
            }

            return false;
        }


        /*Энумиратор возвращаю не свой - это проще и соответствует логике класса. Но это не потокобезопасно. 
         В идеале надо отслеживать время жизни выданных энумираторов и не разрешать редактировать коллекцию пока есть живые энумираторы.
         Но это слишком круто, к тому же слишком не очевидно для программиста - он может просто так вызвать энумиратор и не пользоваться им,
         а зато тем самым по-неведению заблокирует все обращения на модификацию коллекции. К тому же тут возможет дедлок - возьмешь энумиратор
         и не используешь его, а полезешь редактирвоать коллекцию в этом же потоке и всё, дедлок. 
         Для потокобезопасности предусмотрю явные методы, вроде GetLock, GetFree*/
        public IEnumerator<KeyValuePair<KeysPair<TId, TName>, TValue>> GetEnumerator()
        {
            return full_dic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return full_dic.GetEnumerator();
        }
    }


    //Структура составного ключа.
    struct KeysPair<TId, TName>
    {
        public TId id;  //здесь инкапсуляция не к чему
        public TName name;
        public KeysPair(TId id, TName name)
            : this()
        {
            this.id = id;
            this.name = name;
        }

        public override bool Equals(object ob)
        {
            if (ob is KeysPair<TId, TName>)
            {
                KeysPair<TId, TName> c = (KeysPair<TId, TName>)ob;
                return id.Equals(c.id) && name.Equals(c.name);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ name.GetHashCode();   //За конечным пользователем остается проследить, чтобы в пользовательском типе данных был
            //рабочий вариант GetHashCode()
        }

        public override string ToString()   //Для отладки
        {
            return id.ToString() + " "+ name.ToString();
        }

    }
}

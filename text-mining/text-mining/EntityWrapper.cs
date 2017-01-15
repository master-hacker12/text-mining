using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EP;

namespace text_mining
{
    class EntityWrapper
    {
        /// <summary>
        /// Обёртка над семантической сущностью
        /// </summary>

            public EntityWrapper(Referent r)
            {
                Source = r;
            }
        

            public Referent Source;

            /// <summary>
            /// Имя типа сущности
            /// </summary>
            public string TypeName { get { return Source.InstanceOf.Caption; } }
            /// <summary>
            /// Краткое описание
            /// </summary>
            public string Caption { get { return Source.ToString(); } }
        }
 
}

public class EasyBlocks
{
    private List<EasyBlock> Blocks;

    // Constructor with IMyTerminalBlock list
    public EasyBlocks(List<IMyTerminalBlock> TBlocks)
    {
        this.Blocks = new List<EasyBlock>();

        for(int i = 0; i < TBlocks.Count; i++)
        {
            EasyBlock Block = new EasyBlock(TBlocks[i]);
            this.Blocks.Add(Block);
        }
    }

    // Constructor with EasyBlock list
    public EasyBlocks(List<EasyBlock> Blocks)
    {
        this.Blocks = Blocks;
    }

    public EasyBlocks()
    {
        this.Blocks = new List<EasyBlock>();
    }

    // Get number of blocks in list
    public int Count()
    {
        return this.Blocks.Count;
    }

    // Get a specific block from the list
    public EasyBlock GetBlock(int i)
    {
        return this.Blocks[i];
    }

    /*********************/
    /*** Block Filters ***/
    /*********************/

    /*** Interface Filters ***/

    public EasyBlocks WithInterface<T>() where T: class
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            T block = this.Blocks[i].Block as T;

            if(block != null)
            {
                FilteredList.Add(this.Blocks[i]);
            }
        }

        return new EasyBlocks(FilteredList);
    }

    /*** Type Filters ***/

    public EasyBlocks OfType(String Type)
    {
        return TypeFilter("==", Type);
    }

    public EasyBlocks NotOfType(String Type)
    {
        return TypeFilter("!=", Type);
    }

    public EasyBlocks OfTypeLike(String Type)
    {
        return TypeFilter("~", Type);
    }

    public EasyBlocks NotOfTypeLike(String Type)
    {
        return TypeFilter("!~", Type);
    }

    public EasyBlocks OfTypeRegex(String Pattern)
    {
        return TypeFilter("R", Pattern);
    }

    public EasyBlocks NotOfTypeRegex(String Pattern)
    {
        return TypeFilter("!R", Pattern);
    }

    protected EasyBlocks TypeFilter(String op, String Type)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            if(EasyCompare(op, this.Blocks[i].Type(), Type))
            {
                FilteredList.Add(this.Blocks[i]);
            }
        }

        return new EasyBlocks(FilteredList);
    }

    /*** Name Filters ***/

    public EasyBlocks Named(String Name)
    {
        return NameFilter("==", Name);
    }

    public EasyBlocks NotNamed(String Name)
    {
        return NameFilter("!=", Name);
    }

    public EasyBlocks NamedLike(String Name)
    {
        return NameFilter("~", Name);
    }

    public EasyBlocks NotNamedLike(String Name)
    {
        return NameFilter("!~", Name);
    }

    public EasyBlocks NamedRegex(String Pattern)
    {
        return NameFilter("R", Pattern);
    }

    public EasyBlocks NotNamedRegex(String Pattern)
    {
        return NameFilter("!R", Pattern);
    }

    protected EasyBlocks NameFilter(String op, String Name)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            if(EasyCompare(op, this.Blocks[i].Name(), Name))
            {
                FilteredList.Add(this.Blocks[i]);
            }
        }

        return new EasyBlocks(FilteredList);
    }

    /*** Group Filters ***/

    public EasyBlocks InGroupsNamed(String Group)
    {
        return GroupFilter("==", Group);
    }

    public EasyBlocks InGroupsNotNamed(String Group)
    {
        return GroupFilter("!=", Group);
    }

    public EasyBlocks InGroupsNamedLike(String Group)
    {
        return GroupFilter("~", Group);
    }

    public EasyBlocks InGroupsNotNamedLike(String Group)
    {
        return GroupFilter("!~", Group);
    }

    public EasyBlocks InGroupsNamedRegex(String Pattern)
    {
        return GroupFilter("R", Pattern);
    }

    public EasyBlocks InGroupsNotNamedRegex(String Pattern)
    {
        return GroupFilter("!R", Pattern);
    }

    public EasyBlocks GroupFilter(String op, String Group)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        List<IMyBlockGroup> groups = EasyAPI.grid.BlockGroups;
        List<IMyBlockGroup> matchedGroups = new List<IMyBlockGroup>();

        for(int n = 0; n < groups.Count; n++)
        {
            if(EasyCompare(op, groups[n].Name, Group))
            {
                matchedGroups.Add(groups[n]);
            }
        }

        for(int n = 0; n < matchedGroups.Count; n++)
        {
            for(int i = 0; i < this.Blocks.Count; i++)
            {
                IMyTerminalBlock block = this.Blocks[i].Block;

                for(int j = 0; j < matchedGroups[n].Blocks.Count; j++)
                {
                    if(block == matchedGroups[n].Blocks[j])
                    {
                        FilteredList.Add(this.Blocks[i]);
                    }
                }
            }
        }

        return new EasyBlocks(FilteredList);
    }

    /*** Sensor Filters ***/

    public EasyBlocks SensorsActive(bool isActive = true)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            if(this.Blocks[i].Type() == "Sensor" && ((IMySensorBlock)this.Blocks[i].Block).IsActive == isActive)
            {
                FilteredList.Add(this.Blocks[i]);
            }
        }

        return new EasyBlocks(FilteredList);
    }

    public EasyBlocks RoomPressure(String op, Single percent)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            if(this.Blocks[i].RoomPressure(op, percent))
            {
                FilteredList.Add(this.Blocks[i]);
            }
        }

        return new EasyBlocks(FilteredList);
    }


    /*** Advanced Filters ***/

    public EasyBlocks FilterBy(Func<EasyBlock, bool> action)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            if(action(this.Blocks[i]))
            {
                FilteredList.Add(this.Blocks[i]);
            }
        }

        return new EasyBlocks(FilteredList);
    }


    /*** Other ***/

    public EasyBlocks First()
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        if(this.Blocks.Count > 0)
        {
            FilteredList.Add(Blocks[0]);
        }

        return new EasyBlocks(FilteredList);
    }

    public EasyBlocks Add(EasyBlock Block)
    {
        this.Blocks.Add(Block);

        return this;
    }

    public EasyBlocks Plus(EasyBlocks Blocks)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        FilteredList.AddRange(this.Blocks);
        for(int i = 0; i < Blocks.Count(); i++)
        {
            if(!FilteredList.Contains(Blocks.GetBlock(i)))
            {
                FilteredList.Add(Blocks.GetBlock(i));
            }
        }

        return new EasyBlocks(FilteredList);
    }

    public EasyBlocks Minus(EasyBlocks Blocks)
    {
        List<EasyBlock> FilteredList = new List<EasyBlock>();

        FilteredList.AddRange(this.Blocks);
        for(int i = 0; i < Blocks.Count(); i++)
        {
            FilteredList.Remove(Blocks.GetBlock(i));
        }

        return new EasyBlocks(FilteredList);
    }

    public static EasyBlocks operator +(EasyBlocks a, EasyBlocks b)
    {
        return a.Plus(b);
    }

    public static EasyBlocks operator -(EasyBlocks a, EasyBlocks b)
    {
        return a.Minus(b);
    }

    /*** Operations ***/

    public EasyBlocks FindOrFail(string message)
    {
        if(this.Count() == 0) throw new Exception(message);

        return this;
    }

    public EasyBlocks SendMessage(EasyMessage message)
    {
        for(int i = 0; i < this.Blocks.Count; i++)
        {
            this.Blocks[i].SendMessage(message);
        }

        return this;
    }


    public EasyBlocks ApplyAction(String Name)
    {
        for(int i = 0; i < this.Blocks.Count; i++)
        {
            this.Blocks[i].ApplyAction(Name);
        }

        return this;
    }

    public EasyBlocks SetProperty<T>(String PropertyId, T value, int bleh = 0)
    {
        for(int i = 0; i < this.Blocks.Count; i++)
        {
            this.Blocks[i].SetProperty<T>(PropertyId, value);
        }

        return this;
    }

    public T GetProperty<T>(String PropertyId, int bleh = 0)
    {
        return this.Blocks[0].GetProperty<T>(PropertyId);
    }

    public EasyBlocks On()
    {
        for(int i = 0; i < this.Blocks.Count; i++)
        {
            this.Blocks[i].On();
        }

        return this;
    }

    public EasyBlocks Off()
    {
        for(int i = 0; i < this.Blocks.Count; i++)
        {
            this.Blocks[i].Off();
        }

        return this;
    }

    public EasyBlocks Toggle()
    {
        for(int i = 0; i < this.Blocks.Count; i++)
        {
            this.Blocks[i].Toggle();
        }

        return this;
    }

    public EasyInventory Items()
    {
        return new EasyInventory(this.Blocks);
    }

    public string DebugDump(bool throwIt = true)
    {
        String output = "\n";

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            output += this.Blocks[i].Type() + ": " + this.Blocks[i].Name() + "\n";
        }

        if(throwIt)
            throw new Exception(output);
        else
            return output;
    }

    public string DebugDumpActions(bool throwIt = true)
    {
        String output = "\n";

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            output += "[ " + this.Blocks[i].Type() + ": " + this.Blocks[i].Name() + " ]\n";
            output += "*** ACTIONS ***\n";
            List<ITerminalAction> actions = this.Blocks[i].GetActions();

            for(int j = 0; j < actions.Count; j++)
            {
                output += actions[j].Id + ":" + actions[j].Name + "\n";
            }

            output += "\n\n";
        }

        if(throwIt)
            throw new Exception(output);
        else
            return output;
    }

    public string DebugDumpProperties(bool throwIt = true)
    {
        String output = "\n";

        for(int i = 0; i < this.Blocks.Count; i++)
        {
            output += "[ " + this.Blocks[i].Type() + ": " + this.Blocks[i].Name() + " ]\n";
            output += "*** PROPERTIES ***\n";
            List<ITerminalProperty> properties = this.Blocks[i].GetProperties();

            for(int j = 0; j < properties.Count; j++)
            {
                output += properties[j].TypeName + ": " + properties[j].Id + "\n";
            }

            output += "\n\n";
        }

        if(throwIt)
            throw new Exception(output);
        else
            return output;
    }
}

/*** Menus ***/

// Base EasyMenu class
public class EasyMenu
{
    /*** Private Properties ***/
    
    private EasyMenuItem selectedItem; // The selected menu item (child)
    Stack<EasyMenuItem> path;
    
    /*** Constructors ***/
    
    public EasyMenu(string text, EasyMenuItem[] children = null)
    {
        this.path = new Stack<EasyMenuItem>();
        
        this.path.Push(new EasyMenuItem(text, children));
        
        if(this.path.Peek().children.Count > 0)
        {
            this.selectedItem = this.path.Peek().children[0];
        }
    }
    
    /*** Public API ***/
    
    // Move cursor up
    public void Up()
    {
        int location = this.path.Peek().children.IndexOf(this.selectedItem) - 1;
        
        if(location >= 0)
        {
            this.selectedItem = this.path.Peek().children[location];
        }
    }
    
    // Move cursor down
    public void Down()
    {
        int location = this.path.Peek().children.IndexOf(this.selectedItem) + 1;
        
        if(location < this.path.Peek().children.Count)
        {
            this.selectedItem = this.path.Peek().children[location];
        }        
    }
    
    // Choose action
    public void Choose()
    {
        if(this.selectedItem.doAction())
        {
            if(this.selectedItem.children.Count > 0) // go to sub menu if it exists
            {
                this.path.Push(this.selectedItem);
                this.selectedItem = this.path.Peek().children[0];                            
            }
        }
    }
    
    // Back action
    public void Back()
    {
        if(this.path.Count > 1)
        {
            this.path.Pop();
        
            this.selectedItem = this.path.Peek().children[0];            
        }
    }
    
    // Return string of rendered menu
    public string Draw(int textWidth = 40, int textHeight = 7)
    {
        // long clock = DateTime.Now.Ticks; // used to scroll menu text if it is too wide
        string output = "";
        
        var currentItem = this.path.Peek();
        
        if(currentItem == null)
        {
            throw new Exception("Menu: currentItem is null");
        }
            
        output += this.breadcrumbs() + "\n\n";
        
        
        int selected = currentItem.children.IndexOf(this.selectedItem);
        
        int start = 0;
        int length = currentItem.children.Count;
        
        
        if(selected > (textHeight - 2) / 2)
        {
            start = selected - (textHeight - 2) / 2;
        }

        if(length > textHeight - 2)
        {
            length = textHeight - 2;
        }
               
        if(start + length > currentItem.children.Count)
        {
            start = currentItem.children.Count - (textHeight - 2);
            length = textHeight - 2;
        }

        if(start < 0) start = 0;
        
        for(int n = start; (n < (start + length) && n < currentItem.children.Count); n++)
        {
            EasyMenuItem child = currentItem.children[n];
            if(child == this.selectedItem)
            {
                // Todo, if text is too wide, have it scroll horizontally 
                output += "> " + child.GetText() + " <\n";
            }
            else
            {
                output += child.GetText() + "\n";
            }            
        }
        
        return output;
    }
    
    /*** Private Methods ***/
    
    // Returns breadcrumbs for menu title
    private string breadcrumbs()
    {
        if(this.path.Count == 1)
        {
            return this.path.Peek().GetText();
        }
        else
        {
            var item = this.path.Pop();
            
            var s = this.breadcrumbs() + " � " + item.GetText();
            
            this.path.Push(item);
            
            return s;
        }
    }
}

// Holds menu item info
public class EasyMenuItemClass
{
    /*** Private Properties ***/
    
    private Func<EasyMenuItem, bool> chooseAction; // Function to run when item is chosen
    private Func<EasyMenuItem, string> textAction; // Function to update the text for dynamic menu text

    /*** Public Properties ***/
    
    public long uid; // Unique id of this item for comparisons 
    public string Text; // The text that shows up in the menu for this item
    public List<EasyMenuItem> children; // The children items of this item
    
    public static long currentUid = 0; // Used to generate unique ids for each item in a menu
    
    /*** Constructors ***/

    // Standard constructor
    public EasyMenuItemClass(string text, EasyMenuItem[] children = null, Func<EasyMenuItem, bool> chooseAction = null, Func<EasyMenuItem, string> textAction = null)
    {
        // increment unique id
        EasyMenuItemClass.currentUid++;
        
        this.Text = text;
        this.chooseAction = chooseAction;
        this.textAction = textAction;
        this.uid = EasyMenuItemClass.currentUid;

        // Initialize children
        if(children == null)
        {
            this.children = new List<EasyMenuItem>();
        }
        else
        {
            this.children = new List<EasyMenuItem>(children);        
        }

        // If no text action is specified, use the default one
        if(this.textAction == null)
        {
            this.textAction = get_text;    
        }
    }
    
    /*** Methods ***/

    // Default text action which just returns the static text
    private string get_text(EasyMenuItem item)
    {
        return this.Text;
    }
    
    // Get menu text
    public string GetText(EasyMenuItem item)
    {
        return this.textAction(item);
    }
    
    // True means go into sub menu if it exists, false means stay where you are
    public bool doAction(EasyMenuItem item)
    {
        if(this.chooseAction == null)
        {
            return true;
        }
        else
        {
            return this.chooseAction(item);
        }
    }
    
    // Set the static text
    public void SetText(string text)
    {
        this.Text = text;
    }
    
    // Set the dynamic text action
    public void SetTextAction(Func<EasyMenuItem, string> textAction)
    {
        this.textAction = textAction;
    }

}

// This is to work around an issue in the PB where you can't use a user defined class in a List.  It basically just maps the public methods/properties onto the EasyMenuItemClass class
public struct EasyMenuItem : IComparable<EasyMenuItem>
{
    /*** Private Properties ***/
    
    private EasyMenuItemClass item;
    
    /*** Public Properties ***/
    
    public long uid { get {return this.item.uid;} }
    public string Text { get {return this.item.Text;} } // The text that shows up in the menu for this item
    public List<EasyMenuItem> children { get {return this.item.children;} set {this.item.children = value;} } // The children items of this item

    /*** Constructors ***/
    
    public EasyMenuItem(string text, EasyMenuItem[] children = null, Func<EasyMenuItem, bool> chooseAction = null, Func<EasyMenuItem, string> textAction = null)
    {
        this.item = new EasyMenuItemClass(text, children, chooseAction, textAction);
    }
    
    public EasyMenuItem(string text, Func<EasyMenuItem, bool> chooseAction, Func<EasyMenuItem, string> textAction = null) : this(text, null, chooseAction, textAction) {}
    public EasyMenuItem(Func<EasyMenuItem, string> textAction, Func<EasyMenuItem, bool> chooseAction = null) : this("", chooseAction, textAction) {}
    
    /*** Methods ***/

    public string GetText() {return this.item.GetText(this);}
    public bool doAction() {return this.item.doAction(this);}
    public void SetText(string text) {this.item.SetText(text);}
    public void SetTextAction(Func<EasyMenuItem, string> textAction) {this.item.SetTextAction(textAction);}
    
    /*** Operators ***/
    
    public static bool operator ==(EasyMenuItem x, EasyMenuItem y) {return x.uid == y.uid;}    
    public static bool operator !=(EasyMenuItem x, EasyMenuItem y) {return !(x == y);}    
    public int CompareTo(EasyMenuItem otherItem) {return this.GetText().CompareTo(otherItem.GetText());}
}

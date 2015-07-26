# Schrodinger Type
Like the famous [paradox](https://en.wikipedia.org/wiki/Schr%C3%B6dinger%27s_cat), a .NET type that can be any value until you check it.

**Usage**

    var s = new Schrodinger();
    var val = s.Value;
    
**Value** is a dynamic property. Until you don't access it, it can assume any value (all base types, DateTime, DateTimeOffset, TimeSpan, Guid or even object). Once you read it, it will always have the same value.

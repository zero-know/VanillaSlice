using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {{ProjectName}}.ServiceContracts.Models
{
    public readonly record struct SelectOption<T>(T Value, string Text)
    {
        public bool Disabled { get; init; }  // optional
        public string? Group { get; init; }  // optional

        // nice-to-haves
        public static implicit operator SelectOption<T>((T value, string text) x) => new(x.value, x.text);
        public void Deconstruct(out T value, out string text) { value = Value; text = Text; }
    }

    public static class OptionList
    {
        public static List<SelectOption<T>> From<T>(IEnumerable<T> items, Func<T, string> text)
            => items.Select(x => new SelectOption<T>(x, text(x))).ToList();

        public static List<SelectOption<TEnum>> FromEnum<TEnum>() where TEnum : struct, Enum
            => Enum.GetValues<TEnum>().Select(e => new SelectOption<TEnum>(e, e.ToString())).ToList();
    }
}

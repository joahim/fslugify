namespace FSlugify.Tests

open NUnit.Framework
open FSlugify.SlugGenerator
open FSlugify.Builder

type SlugGeneratorTests() =

    [<TestCase("test", "test")>]
    [<TestCase("url with spaces", "url_with_spaces")>]
    [<TestCase("       url to trim    ", "url_to_trim")>]
    [<TestCase("To Lower", "to_lower")>]
    [<TestCase("ToSeparate", "to_separate")>]
    [<TestCase("toSeparate", "to_separate")>]
    [<TestCase("EVERY CHAR CAPSLOCK", "every_char_capslock")>]
    [<TestCase("{with} [symbols)", "with_symbols")>]
    [<TestCase("{with!  #@? symbols)", "with_symbols")>]
    [<TestCase("!£$% symbols at start end !£$%  ", "symbols_at_start_end")>]
    [<TestCase("Test with numbers23", "test_with_numbers23")>]
    [<TestCase("Déjà Vu!", "deja_vu")>]
    member this.``Test slugify with default options`` (input, expectedOutput) =
        let stringSlugified = slugify DefaultSlugGeneratorOptions input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("Test Case", '@', "test@case")>]
    [<TestCase("    MORE   TEST    CASE  ", '#', "more#test#case")>]
    [<TestCase("{With} [Symbols)", '.', "with.symbols")>]
    [<TestCase("Déjà Vu!!!", '§', "deja§vu")>]
    member this.``Test slugify with custom separator`` (input, customSeparator, expectedOutput) =
        let options = { DefaultSlugGeneratorOptions with Separator = customSeparator }
        let stringSlugified = slugify options input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("Test Case", "Test@Case")>]
    [<TestCase("    MORE   TEST    CASE  ", "MORE@TEST@CASE")>]
    [<TestCase("{With} [Symbols)", "With@Symbols")>]
    [<TestCase("DÉJÀ VU!!!", "DEJA@VU")>]
    member this.``Test slugify with lowercase false`` (input, expectedOutput) =
        let options = { DefaultSlugGeneratorOptions with Separator = '@'; Lowercase = false }
        let stringSlugified = slugify options input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("Test | Case", "test_or_case")>]
    [<TestCase("  &  MORE   TEST  &  CASE  ", "and_more_test_and_case")>]
    [<TestCase("{With}⏳[Symbols)", "with_hourglass_symbols")>]
    [<TestCase("DÉJÀ 🤡!!!", "deja_clown")>]
    member this.``Test slugify method with custom map`` (input, expectedOutput) =
        let customMap = [ ("|", " or "); ("&", " and "); ("⏳", " hourglass "); ("🤡", " clown") ]
        let options = { DefaultSlugGeneratorOptions with CustomMap = customMap }
        let stringSlugified = slugify options input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("", "")>]
    [<TestCase("                    ", "")>]
    [<TestCase("B", "b")>]
    [<TestCase("a", "a")>]
    [<TestCase("Ç", "c")>]
    [<TestCase(null, "")>]
    member this.``Test slugify with edge cases`` (input, expectedOutput) =
        let stringSlugified = slugify DefaultSlugGeneratorOptions input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("ښ ۍ", "x_ai")>]
    member this.``Test slugify with Pashto`` (input, expectedOutput) =
        let stringSlugified = slugify DefaultSlugGeneratorOptions input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("Ф Щ ЮЯЁ", "f_shh_yu_ya_yo")>]
    member this.``Test slugify with Russian`` (input, expectedOutput) =
        let stringSlugified = slugify DefaultSlugGeneratorOptions input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("\u0065\u0301", "e")>]
    [<TestCase("\u00e9", "e")>]
    member this.``Test normalized strings`` (input, expectedOutput) =
        let stringSlugified = slugify DefaultSlugGeneratorOptions input
        Assert.AreEqual(expectedOutput, stringSlugified)

    [<TestCase("Test | Case", "Test@or@Case")>]
    [<TestCase("  &  MORE   TEST  &  CASE  ", "and@MORE@TEST@and@CASE")>]
    [<TestCase("{With}⏳[Symbols)", "With@hourglass@Symbols")>]
    [<TestCase("DÉJÀ 🤡!!!", "DEJA@clown")>]
    member this.``Test computation expression builder`` (input, expectedOutput) =
        let customSlugify = slug {
            separator '@'
            lowercase false
            custom_map ("|", " or ")
            custom_map ("&", " and ")
            custom_map ("⏳", " hourglass ")
            custom_map ("🤡", " clown")
        }
        let stringSlugified = customSlugify input
        Assert.AreEqual(expectedOutput, stringSlugified)
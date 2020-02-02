namespace BibleStats.Bible.Domain

type BookId = string

type ChapterId = int16

type VerseId = int16

type Bible = {
   OldTestament:Book list
   NewTestament:Book list
}

and Book = {
   Id:BookId
   Chapters:Chapter list
}

and Chapter = {
   Id:ChapterId
   Verses:Verse list
}

and Verse = {
   Id:VerseId
   Text:string
}

type Section = {
   Book:BookId
   Begin:ChapterId*VerseId
   End:ChapterId*VerseId
}

type Reading = Section list


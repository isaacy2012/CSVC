# CSVC
Categorise CSV cells based on string matching and Regex. By parsing a config file, CSVC automatically categorizes the CSV cells. 

![Screenshot](readMeImages/screenshot.PNG)

# Parameters
```noheader``` means the input CSV file doesn't contain a header, so matching can begin from the first (0 indexed) row

```column NUMLIST``` sets the columns of interest to the list of numbers. e.g ```column 0,1,2``` will mean CSVC searches through columns 0,1, and 2 to attempt to match.

```MODE``` controls how CSVC will write the substitutions. ```append``` simply appends the substitution to the end of the line, ```replace NUM``` replaces column NUM 
with the substitution, and ```inserts NUM QUOTE``` inserts a column with the name QUOTE at index NUM.

# Grammar 
The grammar for the config file is as follows:
```
CONFIG ::= HEADER? COLUMN? MODE CATEGORY+
HEADER ::= header OPTSEMI
COLUMN ::= column NUMLIST OPTSEMI
NUMLIST ::= NUM [, NUM]*
MODE ::= append OPTSEMI replace NUM OPTSEMI | insert NUM QUOTE OPTSEMI
CATEGORY ::= category QUOTE { RULESET* }
RULESET ::= STRINGRULETYPE | REGEXRULETYPE
STRINGRULETYPE ::= RULETYPE { RULE* }
RULETYPE ::= equals | contains
REGEXRULTEYPE ::= matches { REGEXRULE* }
RULE ::= QUOTE OPTSEMI
REGEXRULE ::= REGEX OPTSEMI
QUOTE ::= "[a-zA-Z]"
REGEX ::= ".*"
NUM ::= [0-9]+
OPTSEMI ::= ;?
```

e.g 
```
noheader
column 0,1
replace 1
category "Food And Drink" {
    contains {
        "Rest"
        "Mc Donalds"
        "Maccas"
    }
}
category "Bad" {
    contains {
        "Key"
    }
}
category "Groceries" {
    equals {
        "New World"
    }
    contains {
        "New World"
        "CountDown"
    }
    matches {
        "b.*a"
    }
}
```

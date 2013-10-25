Scheduler
=========
A framework to create schedules for a sports league with an arbitrary set of scheduling rules and preferences
that go beyond a simple round robin scheduler.

By default the scheduler is set up to have the following rules:
- Each team should play roughly the same number of games in each time slot
- Each team should play each other team roughly an even number of times
- Repeat games should not occur before a team has played most other teams

It also supports some situational rules that a coordinator can set up:
- Team X should play in timeslot B on a specified week
- Team X and Team Y should play in consecutive time slots (helpful for parents on different hat teams)
- When Team X and Team Y play against each other, it should be in an earlier/later timeslot.

At present, the league set-up and rules can be modified by tweaking Program.cs, however there are plans to
extend this to be more config file driven.  To add new rules, simply create new IRule implementations that
meet your unique needs.

Background
==========
Creating a schedule of this complexity is not a deterministic task, and so the Scheduler is written as a 
Genetic Algorithm.  Briefly, this technique allows a huge problem space to be explored by utilizing 
pseudo-Darwinian concepts which apply selective pressure to drive towards a better and better solution.

Different iterations of the Scheduler application can and will arrive at different schedules.  An acceptable 
schedule may take quite a bit of time to arrive at.  To aid in determining when an acceptable schedule has
been discovered, diagnostic reports have been provided so that the leading candidateas can be examined in real time.


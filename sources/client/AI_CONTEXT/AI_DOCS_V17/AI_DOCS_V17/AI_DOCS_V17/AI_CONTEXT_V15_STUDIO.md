# AI_CONTEXT_V15_STUDIO

## Studio reality
This is still a prototype-first Unity project. The team is intentionally prioritizing:
- architecture
- gameplay foundations
- maintainability
- clean handoff between AI sessions

over final art production.

Do not push the project toward premature visual polish if the gameplay structure is not yet locked.

## Canonical production mindset at V15
- keep the validated 5v5 runtime intact
- preserve the GameManager-centric match architecture
- preserve the minimap as a passive read-only gameplay UI system
- avoid reopening already solved issues without evidence of regression
- keep future networking in mind even though the project is still local-only
- keep documentation continuity strong enough for a new AI to resume instantly

## What changed since V14
V14 ended with the stabilized GameManager/HUD/end-flow baseline and defined minimap as the next immediate task.

V15 now confirms:
- minimap implemented and validated
- minimap upgraded to V1.1
- local player marker rotation works correctly
- marker clamping is in place
- minimap readability pass completed
- panel placement/background issues diagnosed and fixed
- triangle marker import workflow understood and documented

## Practical truth about the current project phase
The project is still in the **gameplay systems / greybox** phase.
That means:
- primitives and simple UI are acceptable
- art tools like MidJourney / Mushy are not the immediate bottleneck
- design differentiation can be explored conceptually, but should not replace short-term core tasks

## Immediate production priority after V15
The next short-term production step is not the advanced map identity.
It is:
- `1.4 Hero Selection Screen`

Then:
- real Wonder/Core objective
- richer end-of-match screen

## Strategic design lane now documented
A future design lane has been explicitly acknowledged:
- innovative map concepts
- dynamic events
- faction systems
- seasonal meta
- map asymmetry or controlled randomness

This lane is valid, but it is not the current execution priority.

## Important caution for future AI
If a future AI resumes from V15:
- do not claim minimap is still "next step"
- do not skip the fact that minimap V1.1 is already done
- do not collapse the distinction between:
  - short-term production priorities
  - medium/long-term design exploration

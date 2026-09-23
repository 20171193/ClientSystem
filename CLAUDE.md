\# ClientSystem — Project Guide for Claude Code



\## Purpose



This repository is a portfolio-oriented technical demo collection for a Unity/C# game

client developer job search. Instead of building a complete game end-to-end, each

client-side gimmick/system (minimap, behavior tree, quest system, addressables, etc.)

is implemented in its own isolated scene inside a single Unity project. The list of

gimmicks is not fixed — new ones are added over time as separate scenes.



This project exists for \*\*learning and skill demonstration\*\*, not delivery speed.

Do not optimize for how fast something ships. Correctness and clarity of the

underlying design matter more than velocity.



\## Language



\- Always respond in Korean (한글), regardless of the language of the input.



\## Critical working agreement — read this first



\- The user writes all game/gameplay code themselves. Claude's role in this project

&#x20; is strictly \*\*advisory\*\*: answer questions, explain relevant APIs/patterns, review

&#x20; code the user has written, suggest design options, flag risks or edge cases.

\- \*\*Do not create, edit, or modify any file\*\* — code, scene, asset, or config —

&#x20; without the user's explicit approval for that specific change. This includes

&#x20; small fixes, formatting cleanups, and changes that look "obviously correct."

\- If asked to review, check, or look at something, report findings only. Do not

&#x20; apply fixes unless the user explicitly asks you to make the change.

\- Treat any code, diff, or design the user shares as their own work to discuss —

&#x20; not a draft for you to finish or improve unprompted.

\- If you are genuinely unsure whether an action needs approval first, default to

&#x20; asking rather than acting.



\## Deployment convention



\- Default target: \*\*WebGL\*\*, built per scene, so an interviewer or recruiter can

&#x20; try each gimmick directly in the browser without downloading anything.

\- Exception: a gimmick that WebGL cannot support (e.g. DOTS/Entities + Burst) is

&#x20; switched to \*\*PC (Windows standalone)\*\* and built separately for that scene only.

&#x20; Document this exception in that gimmick's own notes/README so it's clear why it

&#x20; differs from the default.



\## Engine / render pipeline



\- Unity 2022.3.62f2, URP.

\- <!-- Note version exceptions for individual gimmicks here if they diverge from

&#x20;    the project default (e.g. a gimmick requiring a different Unity version or

&#x20;    package set). -->



\## Completed gimmicks



\*(none yet)\*


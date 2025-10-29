# Technical Exercise - Lead Developer - Thrillz

## Objective

Design the "Thrillz" game loop with a strong Live experience.

## Game Loop Scope

The exercise consists of implementing a complete game cycle including the following steps:

1. **Entry** - Player entry point into the system
2. **Matchmaking** - Player matching system
3. **Mini-Game** - Core game mechanic
4. **Score** - Score calculation and display
5. **Leaderboard** - Player rankings
6. **Result** - Game result display
7. **Reward/Claim** - Reward attribution and claiming

## Technical Requirements

### Backend API (TypeScript)

You must create a backend API with the following characteristics:

- **Basic authentication**: Implementation of a simple but functional authentication system
- **Strongly typed and structured collections**: Use of TypeScript with strict types
- **Clean architecture**: Organized, maintainable code following best practices

**Suggested Technologies:**
- Node.js with Express or NestJS
- TypeScript (strict mode)
- MySQL database
- JWT for authentication

### Unity Client (C#)

You must create a Unity client with the following characteristics:

- **Strongly typed API integration**: API requests and responses with strict types
- **Clean architecture**: MVC or MVVM of your choice
- **Advanced animations**: Use of DOTween or equivalent for smooth animations

**Unity Requirements:**
- Clearly defined MVC or MVVM architecture
- Clean state management
- Smooth and professional animations (DOTween, Animator, etc.)

## Expected Deliverable

A **functional prototype** demonstrating:

- Complete game loop (all steps from Entry to Reward)
- Convincing Live experience
- Smooth communication between Unity client and backend
- Quality animations
- Clean, well-structured and maintainable code
- Strict types on both backend and client

## Implementation Guidelines

### Project Structure

```
backend/          -> Your TypeScript API
unity-client/     -> Your Unity project
```

### Recommended Steps

1. **Backend first**: Start by defining your API and data contracts
2. **Unity Client**: Implement API integration and architecture
3. **Animations and Polish**: Add animations and Live feeling
4. **Tests**: Ensure the complete cycle works

### Evaluation Criteria

- **Code quality**: Cleanliness, organization, adherence to SOLID principles
- **Architecture**: Clarity of backend and client architecture
- **Typing**: Correct use of TypeScript and C# types
- **User experience**: Fluidity, animations, Live feeling
- **Documentation**: README, relevant code comments

## Important Notes

- **Estimated time**: Take the time needed to produce quality work (2-4 days recommended)
- **Questions**: Don't hesitate to ask questions if certain points are unclear
- **Creativity**: You have the freedom to choose the type of mini-game and visual style
- **Simplicity**: A simple but well-done prototype is better than a complex unfinished project

## Delivery

Once completed:

1. Fork this repository to your own GitHub account
2. Commit your code to your forked repository
3. Make sure to include a README in each folder (backend/ and unity-client/) with:
   - Installation instructions
   - Instructions to run the project
   - Technical choices made
4. Add a `NOTES.md` file at the root with your thoughts, challenges encountered, and possible future improvements
5. Send us the link to your forked repository

---

**Good luck and have fun!**

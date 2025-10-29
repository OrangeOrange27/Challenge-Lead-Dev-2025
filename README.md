# Technical Exercise - Lead Developer - Thrillz

## Objective

🎯 Objective

Testflight : https://testflight.apple.com/join/yE44pvNa

Play Thrillz on TestFlight to fully understand the competitive game loop.
Then recreate the core Thrillz experience using:

✅ The assets provided in this repository
✅ One mini-game of your choice
(can be extremely simple — a Unity asset, a basic mechanic you create, etc.)

Your implementation should reproduce the essential player journey:

- Pick Game

- Enter a Brawl / Match

- Play the Mini-Game

- Submit Score 

- Check Result & Ranking

- Claim Reward

The primary objective is to demonstrate your ability to build a smooth, live-feeling competitive flow between backend and Unity — clean architecture, strongly typed API integration, and polished UX.

Please note that Thrillz brawls are asynchronous:
Players do not need to be online or playing at the same time for a brawl to happen.

A player can:

Enter a brawl at any moment

Play their game independently

Submit their score whenever they finish

The backend stores their result and later matches it against other players’ scores who join the same brawl. Once all participants have played (or a timer/condition is met), the system:

- Calculates the ranking
- Determines the winner
- Makes rewards claimable

This approach creates a real-time competitive feeling without requiring synchronous multiplayer networking.
## Game Loop Scope

The exercise consists of implementing a complete game cycle including the following steps:

1. **Entry** - Player entry point into the system
2. **Matchmaking** - Player matching system
3. **Mini-Game** - Core game mechanic
4. **Score** - Score calculation and display
5. **Leaderboard** - Player rankings
6. **Result** - Game result display
7. **Reward/Claim** - Reward attribution and claiming

**Important Note:** The games are **asynchronous**. Players are not playing in real-time against each other. Instead, they play individually, and their scores are compared afterwards on the leaderboard. The matchmaking system groups players together for competition, but the actual gameplay happens independently for each player.

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
- Clean state management (proper separation of concerns, no singletons abuse, clear data flow)
- Smooth and professional animations (DOTween, Animator, etc.)

## Design Assets

UI/UX design assets are available on Figma:

[Technical Test Assets - Figma](https://www.figma.com/design/vgveBFBH8Dl2mNMK0IWqCB/Technical-test-Assets?node-id=0-1&t=4jbe5sXNrcO2Kk0k-1)

You can use these assets for your Unity client implementation.

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

### Evaluation Criteria

- **Code quality**: Cleanliness, organization, adherence to SOLID principles
- **Architecture**: Clarity of backend and client architecture
- **Typing**: Correct use of TypeScript and C# types
- **User experience**: Fluidity, animations, Live feeling
- **Documentation**: README, relevant code comments

## Important Notes

- **Estimated time**: Take the time needed to produce quality work (2-4 days recommended)
- **Questions**: Don't hesitate to ask questions if certain points are unclear
- **Mini-Game Choice**: You have complete freedom to choose the type of mini-game you want to implement. This choice will **not be evaluated**. We recommend keeping it simple (e.g., tap game, simple puzzle, quick reflex game, you can use an asset) to focus on the architecture and game loop implementation
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

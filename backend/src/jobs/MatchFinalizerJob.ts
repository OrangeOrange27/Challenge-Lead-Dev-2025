import { RewardService } from "../services/RewardService";
import { MatchState } from "../models/enums";
import { CONFIG } from "../config/constants";
import { MatchStorage } from "../storages/MatchStorage";

export class MatchFinalizerJob {
  private readonly matches = new MatchStorage();
  private readonly rewardService = new RewardService();
  private intervalId: NodeJS.Timeout | null = null;
  private isRunning = false;

  start(): void {
    if (this.intervalId) return;

    console.log(
        `[MatchFinalizerJob] Started with interval ${CONFIG.FINALIZATION_JOB_INTERVAL_MS}ms`
    );

    this.runSafely();

    this.intervalId = setInterval(() => this.runSafely(), CONFIG.FINALIZATION_JOB_INTERVAL_MS);
  }

  stop(): void {
    if (!this.intervalId) return;
    clearInterval(this.intervalId);
    this.intervalId = null;
    console.log("[MatchFinalizerJob] Stopped");
  }

  private async runSafely(): Promise<void> {
    if (this.isRunning) return;
    this.isRunning = true;

    try {
      await this.run();
    } catch (err) {
      console.error("[MatchFinalizerJob] Run error:", err);
    } finally {
      this.isRunning = false;
    }
  }

  private async run(): Promise<void> {
    const matchesToFinalize = await this.findMatchesToFinalize();
    if (matchesToFinalize.length === 0) {
      console.log("[MatchFinalizerJob] No matches to finalize");
      return;
    }

    console.log(`[MatchFinalizerJob] Finalizing ${matchesToFinalize.length} matches`);

    for (const match of matchesToFinalize) {
      await this.finalizeMatch(match.id);
    }
  }

  private async findMatchesToFinalize() {
    const allMatches = await this.matches.getMatchesToFinalize();

    const matchesToFinalize = [];
    for (const match of allMatches) {
      if (match.matchState === MatchState.COMPLETE) continue;

      const entryCount = await this.matches.getParticipantCount(match.id);
      if (entryCount >= match.mode.maxPlayers) {
        matchesToFinalize.push(match);
      }
    }
    return matchesToFinalize;
  }

  private async finalizeMatch(matchId: string): Promise<void> {
    console.log(`[MatchFinalizerJob] Finalizing match ${matchId}`);

    try {
      const finalizedAt = new Date().toISOString();
      await this.matches.updateState(matchId, MatchState.COMPLETE, finalizedAt);
      await this.rewardService.assignRewards(matchId);

      console.log(`[MatchFinalizerJob] Finalized match ${matchId}`);
    } catch (err) {
      console.error(`[MatchFinalizerJob] Error finalizing match ${matchId}:`, err);
    }
  }
}
import jwt, {SignOptions} from "jsonwebtoken";
import {CONFIG} from "../config/constants";
import {PlayersStorage} from "../storages/PlayersStorage";

export class AuthService {
  private players: PlayersStorage;

  constructor() {
    this.players = new PlayersStorage();
  }

  async loginWithDevice(
      deviceId: string
  ): Promise<{ token: string; playerId: string }> {
    if (!deviceId || deviceId.trim().length === 0) {
      throw new Error("Device ID is missing or invalid.");
    }

    let player = await this.players.getByDevice(deviceId);

    if (!player) {
      player = await this.players.registerNew(deviceId);
      console.log("Created new player: ${player.id} for device: ${deviceId}");
    }

    const signOptions: SignOptions = {
      expiresIn: CONFIG.TOKEN_TTL as any,
    };
    const token = jwt.sign(
        {playerId: player.id, deviceId: player.deviceId},
        CONFIG.TOKEN_SECRET,
        signOptions
    );

    return {token, playerId: player.id};
  }

  verifyToken(token: string): { playerId: string; deviceId: string } {
    try {
      return jwt.verify(token, CONFIG.TOKEN_SECRET) as {
        playerId: string;
        deviceId: string;
      };
    } catch (error) {
      throw new Error("Invalid or expired token");
    }
  }
}
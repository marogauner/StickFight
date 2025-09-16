using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static StickFight.src.entities.Entity;
using static StickFight.src.entities.Player;

namespace StickFight.src;

internal class AnimationManager(
    Texture2D idle,
    Texture2D walk = null,
    Texture2D falling = null,
    Texture2D jumpTexture = null,
    Texture2D doublejumpTexture = null,
    Texture2D attackTexture = null,
    int animationSpeed = 6)
{
    private Animation idleAnimation = new Animation(4, 16, 16, idle, animationSpeed);
    private Animation walkAnimation = new Animation(6, 16, 16, walk, animationSpeed);
    private Animation fallAnimation = new Animation(3, 16, 16, falling, animationSpeed);
    private Animation jumpAnimation = new Animation(3, 16, 16, jumpTexture, animationSpeed);
    private Animation doublejumpAnimation = new Animation(3, 16, 16, doublejumpTexture, animationSpeed);
    private Animation attackAnimation = new Animation(4, 16, 16, attackTexture, animationSpeed);
    private PlayerStates currentAnimation = PlayerStates.Idle;

    // Textures
    private Texture2D idle_spritesheet = idle;
    private Texture2D walk_spritesheet = walk;
    private Texture2D jump_spritesheet = jumpTexture;
    private Texture2D falling_spritesheet = falling;
    private Texture2D doublejump_spritesheet = doublejumpTexture;
    private Texture2D attack_spritesheet = attackTexture;

    // PlayerInfo
    private int playerRemainingjumps;
    public bool playerIsAttacking;
    public void Update(PlayerStates state, int remainingJumps)
    {
        currentAnimation = state;
        playerRemainingjumps = remainingJumps;
        switch (state)
        {
            case PlayerStates.Idle:
                idleAnimation.Update();
                break;
            case PlayerStates.Walk:
                walkAnimation.Update();
                break;
            case PlayerStates.Jump:
                if (playerRemainingjumps == 0)
                {
                    doublejumpAnimation.Update();
                }
                else
                {
                    jumpAnimation.Update();
                }
                break;
            case PlayerStates.Fall:
                fallAnimation.Update();
                break;
            default:
                break;
        }

        // Attack
        if (playerIsAttacking)
        {
            attackAnimation.Update();
            if (attackAnimation.animationFinished)
            {
                playerIsAttacking = false;
            }
        }
    }

    public Rectangle GetFrame()
    {
        if (playerIsAttacking)
        {
            return attackAnimation.GetFrame();
        }
        return currentAnimation switch
            {
                PlayerStates.Idle => idleAnimation.GetFrame(),
                PlayerStates.Walk => walkAnimation.GetFrame(),
                PlayerStates.Jump => playerRemainingjumps == 0 ? doublejumpAnimation.GetFrame() : jumpAnimation.GetFrame(),
                PlayerStates.Fall => fallAnimation.GetFrame(),
                _ => idleAnimation.GetFrame(),
            };
    }

    public Texture2D GetTexture()
    {
        if (playerIsAttacking)
        {
            return attack_spritesheet;
        }
        return currentAnimation switch
        {
            PlayerStates.Idle => idle_spritesheet,
            PlayerStates.Walk => walk_spritesheet,
            PlayerStates.Jump => playerRemainingjumps == 0 ? doublejump_spritesheet : jump_spritesheet,
            PlayerStates.Fall => falling_spritesheet,
            _ => idle_spritesheet,
        };
    }

    public void StartAttackAnimation()
    {
        playerIsAttacking = true;
        attackAnimation.ResetAnimation();
    }
}

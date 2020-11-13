using System.Collections.Generic;

public class GameHistory 
{

    public static LinkedList<GameState> gameStates = new LinkedList<GameState>();

    public static GameState recentState;


    public static int size = 10;

    public static GameState GetPredictedState(int tick)
    {
        if (tick > recentState.tick)
        {
            //Debug.Log("Prediction creation started");
            GameState state=new GameState(recentState);
            state.tick = tick;
            /*for(int i = 0; i < state.redTeamState.Count; i++)
            {
                if (state.redTeamState[i].movementPressed)
                {
                    state.redTeamState[i].movementPressed = false;
                    float ay=state.redTeamState[i].angle[1];
                    float dist = 15 * 0.2f;
                    state.redTeamState[i].position[0] = state.redTeamState[i].position[0] + dist * Mathf.Cos(ay);
                    state.redTeamState[i].position[2] = state.redTeamState[i].position[2] + dist * Mathf.Sin(ay);
                }
            }
            for (int i = 0; i < state.blueTeamState.Count; i++)
            {
                if (state.blueTeamState[i].movementPressed)
                {
                    state.blueTeamState[i].movementPressed = false;
                    float ay = state.blueTeamState[i].angle[1];
                    float dist = 15 * 0.2f;
                    state.blueTeamState[i].position[0] = state.blueTeamState[i].position[0] + dist * Mathf.Cos(ay);
                    state.blueTeamState[i].position[2] = state.blueTeamState[i].position[2] + dist * Mathf.Sin(ay);
                }
            }*/
            AddGameState(state);
            //Debug.Log("Prediction State Created");
        }
        return recentState;
    }
    
    public static void AddGameState(GameState state)
    {
            if (size == gameStates.Count)
            {
                gameStates.RemoveFirst();
            }
            gameStates.AddLast(state);
            recentState = state;
            //Debug.Log("Game State Added");
    }

    public static int CheckClientState(ClientState state)
    {
        //Debug.Log(state.playerId+":"+state.tick + ":" + recentState.tick);
        if (state.tick < recentState.tick - size)
        {
            return -1;
        }
        else if (state.tick == recentState.tick || state.tick-recentState.tick==1) 
        {
            //Debug.Log("tick matched");
            if (state.team == "blue")
            {
                ClientState tempState = new ClientState(recentState.blueTeamState[state.playerId]);
                tempState.CompareAndCopy(state);
                recentState.blueTeamState[state.playerId] = tempState;
            }
            else
            {
                ClientState tempState = new ClientState(recentState.redTeamState[state.playerId]);
                tempState.CompareAndCopy(state);
                recentState.redTeamState[state.playerId] = tempState;
            }
            return 2;
        }
        else
        {
            LinkedListNode<GameState> gsn = gameStates.First;
            while(gsn!=null)
            {
                if (gsn.Value.tick == state.tick)
                {
                    if (state.team == "blue")
                    {
                        if (gsn.Value.blueTeamState[state.playerId].CompareState(state))
                        {
                            ClientState tempState = new ClientState(gsn.Value.blueTeamState[state.playerId]);
                            tempState.CompareAndCopy(state);
                            /*if (tempState.movementPressed)
                            {
                                tempState.movementPressed = false;
                                float ay = tempState.angle[1];
                                float dist = 15 * 0.2f;
                                tempState.position[0] = tempState.position[0] + dist * Mathf.Cos(ay);
                                tempState.position[2] = tempState.position[2] + dist * Mathf.Sin(ay);
                            }*/
                            recentState.blueTeamState[tempState.playerId] = tempState;
                            return 3;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    else
                    {
                        if (gsn.Value.redTeamState[state.playerId].CompareState(state))
                        {
                            ClientState tempState = new ClientState(gsn.Value.redTeamState[state.playerId]);
                            tempState.CompareAndCopy(state);
                            /*if (tempState.movementPressed)
                            {
                                tempState.movementPressed = false;
                                float ay = tempState.angle[1];
                                float dist = 15 * 0.2f;
                                tempState.position[0] = tempState.position[0] + dist * Mathf.Cos(ay);
                                tempState.position[2] = tempState.position[2] + dist * Mathf.Sin(ay);
                            }*/
                            recentState.redTeamState[tempState.playerId] = tempState;
                            return 3;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    
                }
                gsn = gsn.Next;
            }
        }
        return 5;
    }

    public static LinkedListNode<GameState> GetGameState(int tick)
    {
        if (tick > recentState.tick - 10 && tick<recentState.tick)
        {
            bool found = false;
            LinkedListNode<GameState> cn = gameStates.Last;
            while (cn != null)
            {
                if (cn.Value.tick == tick)
                {
                    found = true;
                    break;
                }
                cn = cn.Previous;
            }
            if (found)
            {
                return cn;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public static void AddAutoGadgetState(AutoGadgetState gadgetState,string team,string playerId)
    {
        gadgetState.tick = recentState.tick;
        if (team == "blue")
        {   
            recentState.blueTeamState[playerId].autoGadgetStates.Add(gadgetState.id, gadgetState);
        }
        else
        {
            recentState.redTeamState[playerId].autoGadgetStates.Add(gadgetState.id, gadgetState);
        }
    }
}

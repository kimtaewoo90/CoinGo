

def rsi_upbit(code):

    import pandas as pd
    import requests


    url = "https://api.upbit.com/v1/candles/minutes/"+str(3)
    querystring = {"market" : code, "count" : "200"}
    response = requests.request("GET", url, params=querystring)
    data = response.json()
    df =pd.DataFrame(data)
    df=df.reindex(index=df.index[::-1]).reset_index()
    nrsi=rsi_calc(df, 14).iloc[-1]

    return nrsi


def rsi_calc(ohlc, period):

    import pandas as pd

    ohlc["trade_price"] = ohlc["trade_price"]
    delta = ohlc["trade_price"].diff()
    gains, declines = delta.copy(), delta.copy()
    gains[gains < 0] = 0
    declines[declines > 0] = 0

    _gain = gains.ewm(com=(period - 1), min_periods=period).mean()
    _loss = declines.abs().ewm(com=(period - 1), min_periods=period).mean()

    RS = _gain / _loss
    return pd.Series(100 - (100 / (1 + RS)), name="RSI")
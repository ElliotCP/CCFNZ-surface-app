from flask import Flask, render_template, request

app = Flask(__name__)

@app.route('/')
@app.route('/index', methods=['GET'])
def index():
    amount = request.args.get('amount')
    return render_template("mainpage.html", amount=amount)


if __name__ == '__main__' :
	app.run(debug = True, port = 8080, host='127.0.0.1')
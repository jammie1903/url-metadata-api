class TestUrlForm extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      url: '',
      error: '',
      output: 'The response will appear here'
    }
  }

  onSubmit = e => {
    e.preventDefault();

    if (!this.state.url) {
      this.setState({ error: 'Please enter a url' });
      return;
    }

    if (!this.urlInput.checkValidity()) {
      this.setState({ error: 'Please enter a valid url' });
      return;
    }

    this.setState({ error: '' });

    fetch(`/api/metadata?url=${this.state.url}&all=true`)
      .then(response => response.json())
      .then(json => this.setState({ output: JSON.stringify(json, null, ' ') }))
      .catch(() => this.setState({ output: 'An error occured whilst trying to retrieve the metadata' }));
  }

  onUrlChange = e => {
    this.setState({ url: e.target.value });
  }

  render() {
    return (
      <form id="test-url-form" noValidate onSubmit={this.onSubmit}>
        <label htmlFor="url-input">Enter a Url:</label>
        <br />
        <div className='input-container'>
          <input ref={i => { this.urlInput = i }} required type="url" value={this.state.url} id="url-input" name="url" onChange={this.onUrlChange} />
          <button type="submit" className="button">Submit</button>
        </div>
        <span className='error'>{this.state.error}</span>
        <pre>
          <code id="output">
            {this.state.output}
          </code>
        </pre>
      </form>
    );
  }
}

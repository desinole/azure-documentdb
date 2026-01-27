# Slides

This folder contains the presentation slides for the Azure DocumentDB talk.

## Contents

- **Marp Presentation** - `*.md` files in Marp format
- **Images** - `img/` folder for presentation images
- Presentation materials covering Azure DocumentDB features, architecture, and use cases
- Speaker notes and additional resources

## Format

Slides are organized to cover:
1. Introduction to Azure DocumentDB
2. Key features and capabilities
3. Architecture overview
4. Use cases and best practices
5. Demos and live coding references

## Building the Presentation

The slides are written in Marp format and automatically built to HTML, PDF, and PPTX formats via GitHub Actions.

### Local Building

To build locally, install Marp CLI:

```bash
npm install -g @marp-team/marp-cli
```

Then build the presentation:

```bash
# HTML
marp slides.md -o index.html

# PDF
marp slides.md --pdf -o presentation.pdf

# PPTX
marp slides.md --pptx -o presentation.pptx
```

## Viewing

- **Online**: The presentation is automatically published to GitHub Pages when you push changes
- **Local**: Open the built HTML file in a browser or use Marp VS Code extension

## Images

Add presentation images to the `img/` folder and reference them in your slides:

```markdown
![Image description](img/your-image.png)
```
